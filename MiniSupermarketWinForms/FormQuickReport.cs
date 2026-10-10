using System;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public class FormQuickReport : Form
    {
        private const string ApiBaseAddress = "http://localhost:5000/api/";

        private DateTimePicker dtpDate;
        private Button btnRunReport;
        private Label lblTotalOrders;
        private Label lblTotalRevenue;
        private Label lblBestSeller;
        private Label lblNote;

        public FormQuickReport()
        {
            InitializeComponent();

            // Lớp bảo vệ cấp Action: chỉ Admin mới bấm được nút xem báo cáo
            btnRunReport.Enabled = string.Equals(
                SessionManager.CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase);
        }

        // ===== Dựng giao diện bằng code =====
        private void InitializeComponent()
        {
            this.Text = "Báo cáo doanh thu nhanh";
            this.Size = new Size(900, 500);
            this.BackColor = Color.FromArgb(244, 245, 247);

            // --- Thanh chọn ngày ---
            var panelTop = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0), BackColor = Color.White };
            var lblDate = new Label
            {
                Text = "Chọn ngày:",
                AutoSize = true,
                Location = new Point(20, 25),
                Font = new Font("Segoe UI", 10F)
            };
            dtpDate = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(110, 21),
                Width = 140,
                Font = new Font("Segoe UI", 10F)
            };
            btnRunReport = new Button
            {
                Text = "Xem doanh thu ca",
                Location = new Point(270, 18),
                Size = new Size(170, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(41, 100, 180),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };
            btnRunReport.FlatAppearance.BorderSize = 0;
            btnRunReport.Click += btnRunReport_Click;
            panelTop.Controls.AddRange(new Control[] { lblDate, dtpDate, btnRunReport });

            // --- 3 thẻ số liệu ---
            var tableCards = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(10)
            };
            tableCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            tableCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            tableCards.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableCards.Controls.Add(CreateCard("TỔNG SỐ HÓA ĐƠN", Color.FromArgb(41, 128, 185), out lblTotalOrders), 0, 0);
            tableCards.Controls.Add(CreateCard("TỔNG DOANH THU", Color.FromArgb(39, 174, 96), out lblTotalRevenue), 1, 0);
            tableCards.Controls.Add(CreateCard("MẶT HÀNG BÁN CHẠY NHẤT", Color.FromArgb(211, 84, 0), out lblBestSeller), 2, 0);

            // --- Dòng ghi chú ---
            lblNote = new Label
            {
                Dock = DockStyle.Fill,
                Text = "",
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9F, FontStyle.Italic),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };

            // --- Khung chia hàng: thanh ngày / thẻ / ghi chú / khoảng trống ---
            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 4 };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 70F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 200F));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            root.Controls.Add(panelTop, 0, 0);
            root.Controls.Add(tableCards, 0, 1);
            root.Controls.Add(lblNote, 0, 2);
            this.Controls.Add(root);
        }

        private Panel CreateCard(string caption, Color color, out Label valueLabel)
        {
            var card = new Panel { Dock = DockStyle.Fill, BackColor = color, Margin = new Padding(10) };
            valueLabel = new Label
            {
                Dock = DockStyle.Fill,
                Text = "--",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoEllipsis = true
            };
            var lblCaption = new Label
            {
                Dock = DockStyle.Top,
                Height = 45,
                Text = caption,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            card.Controls.Add(valueLabel);
            card.Controls.Add(lblCaption);
            return card;
        }

        // ===== Gọi API báo cáo =====
        private HttpClient GetAuthenticatedClient()
        {
            var client = new HttpClient { BaseAddress = new Uri(ApiBaseAddress) };

            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            }

            return client;
        }

        private async void btnRunReport_Click(object sender, EventArgs e)
        {
            // Kiểm tra quyền lần nữa ngay tại hành động bấm nút
            if (!string.Equals(SessionManager.CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Bạn không có quyền xem dữ liệu tài chính của siêu thị!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            try
            {
                using var client = GetAuthenticatedClient();
                string date = dtpDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                var response = await client.GetAsync($"reports/quick?date={date}");

                if (response.StatusCode == HttpStatusCode.Forbidden ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    MessageBox.Show("Máy chủ từ chối: tài khoản không đủ quyền xem báo cáo!", "Từ chối",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Không lấy được báo cáo! Mã lỗi: " + (int)response.StatusCode,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var report = await response.Content.ReadFromJsonAsync<QuickReportDto>();
                if (report == null) return;

                lblTotalOrders.Text = report.TotalOrders.ToString("N0");
                lblTotalRevenue.Text = $"{report.TotalRevenue:N0} đ";
                lblBestSeller.Text = report.BestSeller;
                lblNote.Text = report.IsSampleData
                    ? "* Dữ liệu mẫu cho ngày " + report.Date + " (chưa có bảng hóa đơn)."
                    : "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối đến Server!\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class QuickReportDto
    {
        public string Date { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public string BestSeller { get; set; } = string.Empty;
        public bool IsSampleData { get; set; }
    }
}