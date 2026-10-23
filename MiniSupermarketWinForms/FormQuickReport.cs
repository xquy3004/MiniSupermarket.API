using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public class FormQuickReport : Form
    {
        private const string ApiBaseAddress = "http://localhost:5000/api/";

        // ===== 1. Toolbar lọc thời gian =====
        private Panel pnlHeader;
        private Label lblFromDate;
        private DateTimePicker dtpFrom;
        private Label lblToDate;
        private DateTimePicker dtpTo;
        private ComboBox cboPresets;
        private Button btnViewReport;

        // ===== 2. Thẻ KPI Tổng hợp (4 thẻ tự co giãn đều 25%) =====
        private TableLayoutPanel pnlKpi;
        private Label lblTotalOrdersVal;
        private Label lblTotalRevenueVal;
        private Label lblAvgOrderVal;
        private Label lblTotalPointsVal;

        // ===== 3. Khung bảng biểu dữ liệu chính (2 card cân đối 50% - 50%) =====
        private TableLayoutPanel pnlContent;
        private Panel pnlLeftCard;
        private Panel pnlRightCard;

        private Label lblTopProductTitle;
        private DataGridView dgvTopProducts;

        private Label lblRevenueTitle;
        private DataGridView dgvRevenue;

        public FormQuickReport()
        {
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.F5)
                {
                    e.Handled = true;
                    LoadReportAsync();
                }
            };

            InitializeComponent();
            this.Load += (s, e) => LoadReportAsync();
        }

        private void InitializeComponent()
        {
            this.Text = "Báo cáo Doanh thu";
            this.Size = new Size(1180, 680);
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.Font = new Font("Segoe UI", 9.5F);
            this.Padding = new Padding(12);

            // ==========================================
            // 1. THANH LỌC THỜI GIAN (TOOLBAR)
            // ==========================================
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(0, 0, 0, 8),
                BackColor = Color.Transparent
            };

            lblFromDate = new Label
            {
                Text = "Từ ngày:",
                Location = new Point(0, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            dtpFrom = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(62, 4),
                Width = 105,
                Font = new Font("Segoe UI", 9.5F),
                Value = new DateTime(DateTime.Today.Year, 1, 1)
            };

            lblToDate = new Label
            {
                Text = "Đến ngày:",
                Location = new Point(182, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            dtpTo = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Location = new Point(252, 4),
                Width = 105,
                Font = new Font("Segoe UI", 9.5F),
                Value = DateTime.Today
            };

            cboPresets = new ComboBox
            {
                Location = new Point(374, 4),
                Size = new Size(140, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5F)
            };
            cboPresets.Items.AddRange(new object[] { "Toàn bộ thời gian", "Hôm nay", "7 ngày gần nhất", "Tháng này" });
            cboPresets.SelectedIndex = 0;
            cboPresets.SelectedIndexChanged += cboPresets_SelectedIndexChanged;

            btnViewReport = new Button
            {
                Text = "Xem báo cáo",
                Location = new Point(526, 3),
                Size = new Size(115, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnViewReport.FlatAppearance.BorderSize = 0;
            btnViewReport.Click += (s, e) => LoadReportAsync();

            pnlHeader.Controls.AddRange(new Control[]
            {
                lblFromDate, dtpFrom, lblToDate, dtpTo, cboPresets, btnViewReport
            });

            // ==========================================
            // 2. DẢI THẺ KPI (4 THẺ CO GIÃN ĐỀU 25%)
            // ==========================================
            pnlKpi = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 64,
                Padding = new Padding(0, 0, 0, 8),
                Margin = new Padding(0),
                BackColor = Color.Transparent,
                RowCount = 1,
                ColumnCount = 4
            };
            pnlKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            pnlKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            pnlKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            pnlKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            pnlKpi.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var cardKpi1 = CreateKpiCard("TỔNG SỐ HÓA ĐƠN", "-- đơn", Color.FromArgb(37, 99, 235), out lblTotalOrdersVal);
            var cardKpi2 = CreateKpiCard("TỔNG DOANH THU", "-- đ", Color.FromArgb(16, 149, 93), out lblTotalRevenueVal);
            var cardKpi3 = CreateKpiCard("HÓA ĐƠN TRUNG BÌNH", "-- đ", Color.FromArgb(217, 119, 6), out lblAvgOrderVal);
            var cardKpi4 = CreateKpiCard("TỔNG ĐIỂM TÍCH LŨY", "-- điểm", Color.FromArgb(124, 58, 237), out lblTotalPointsVal);

            pnlKpi.Controls.Add(cardKpi1, 0, 0);
            pnlKpi.Controls.Add(cardKpi2, 1, 0);
            pnlKpi.Controls.Add(cardKpi3, 2, 0);
            pnlKpi.Controls.Add(cardKpi4, 3, 0);

            // ==========================================
            // 3. KHUNG BẢNG BIỂU DỮ LIỆU (CÂN ĐỐI 50% - 50%)
            // ==========================================
            pnlContent = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                Padding = new Padding(0),
                BackColor = Color.Transparent,
                RowCount = 1,
                ColumnCount = 2
            };
            pnlContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            pnlContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            pnlContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Card Trái: TOP SẢN PHẨM BÁN CHẠY NHẤT
            pnlLeftCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12),
                Margin = new Padding(0, 0, 6, 0)
            };
            pnlLeftCard.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, pnlLeftCard.ClientRectangle,
                    Color.FromArgb(226, 232, 240), ButtonBorderStyle.Solid);
            };

            lblTopProductTitle = new Label
            {
                Text = "TOP SẢN PHẨM BÁN CHẠY NHẤT",
                Dock = DockStyle.Top,
                Height = 32,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };

            dgvTopProducts = CreateStandardGrid();
            dgvTopProducts.Dock = DockStyle.Fill;

            pnlLeftCard.Controls.Add(dgvTopProducts);
            pnlLeftCard.Controls.Add(lblTopProductTitle);

            // Card Phải: BẢNG DOANH THU
            pnlRightCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12),
                Margin = new Padding(6, 0, 0, 0)
            };
            pnlRightCard.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, pnlRightCard.ClientRectangle,
                    Color.FromArgb(226, 232, 240), ButtonBorderStyle.Solid);
            };

            lblRevenueTitle = new Label
            {
                Text = "BẢNG DOANH THU",
                Dock = DockStyle.Top,
                Height = 32,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };

            dgvRevenue = CreateStandardGrid();
            dgvRevenue.Dock = DockStyle.Fill;

            pnlRightCard.Controls.Add(dgvRevenue);
            pnlRightCard.Controls.Add(lblRevenueTitle);

            pnlContent.Controls.Add(pnlLeftCard, 0, 0);
            pnlContent.Controls.Add(pnlRightCard, 1, 0);

            // Ghép toàn bộ vào Form
            this.Controls.Add(pnlContent);
            this.Controls.Add(pnlKpi);
            this.Controls.Add(pnlHeader);
        }

        private DataGridView CreateStandardGrid()
        {
            var grid = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 34,
                GridColor = Color.FromArgb(241, 245, 249)
            };
            grid.RowTemplate.Height = 32;

            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                SelectionBackColor = Color.FromArgb(241, 245, 249),
                SelectionForeColor = Color.FromArgb(51, 65, 85)
            };
            grid.EnableHeadersVisualStyles = false;

            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 9F),
                SelectionBackColor = Color.FromArgb(224, 231, 255),
                SelectionForeColor = Color.FromArgb(15, 23, 42)
            };

            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(30, 41, 59)
            };

            return grid;
        }

        private Panel CreateKpiCard(string title, string initialValue, Color accentColor, out Label lblVal)
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Margin = new Padding(4, 0, 4, 0),
                Padding = new Padding(12, 6, 12, 6)
            };
            pnl.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, pnl.ClientRectangle,
                    Color.FromArgb(226, 232, 240), ButtonBorderStyle.Solid);
                using (var brush = new SolidBrush(accentColor))
                {
                    e.Graphics.FillRectangle(brush, 0, 0, 4, pnl.Height);
                }
            };

            var lblT = new Label
            {
                Text = title,
                Location = new Point(12, 6),
                AutoSize = true,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            lblVal = new Label
            {
                Text = initialValue,
                Location = new Point(12, 24),
                AutoSize = true,
                Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };

            pnl.Controls.AddRange(new Control[] { lblT, lblVal });
            return pnl;
        }

        private void cboPresets_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sel = cboPresets.SelectedItem?.ToString() ?? "";
            DateTime now = DateTime.Today;

            if (sel == "Hôm nay")
            {
                dtpFrom.Value = now;
                dtpTo.Value = now;
            }
            else if (sel == "7 ngày gần nhất")
            {
                dtpFrom.Value = now.AddDays(-6);
                dtpTo.Value = now;
            }
            else if (sel == "Tháng này")
            {
                dtpFrom.Value = new DateTime(now.Year, now.Month, 1);
                dtpTo.Value = now;
            }
            else // Toàn bộ thời gian
            {
                dtpFrom.Value = new DateTime(now.Year, 1, 1);
                dtpTo.Value = now;
            }

            LoadReportAsync();
        }

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

        // ==========================================
        // GỌI API BÁO CÁO DỮ LIỆU THẬT
        // ==========================================
        private async void LoadReportAsync()
        {
            if (!string.Equals(SessionManager.CurrentRole, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Bạn không có quyền xem dữ liệu tài chính của siêu thị!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            btnViewReport.Enabled = false;
            btnViewReport.Text = "Đang tải...";

            try
            {
                using var client = GetAuthenticatedClient();

                string sel = cboPresets.SelectedItem?.ToString() ?? "";
                string queryParams = "";

                if (sel != "Toàn bộ thời gian")
                {
                    string fDate = dtpFrom.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    string tDate = dtpTo.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                    queryParams = $"?fromDate={fDate}&toDate={tDate}";
                }

                // 1. Lấy chỉ số tổng hợp KPI
                var quickRes = await client.GetFromJsonAsync<QuickReportResultDto>($"reports/quick{queryParams}");
                if (quickRes != null)
                {
                    lblTotalOrdersVal.Text = $"{quickRes.TotalOrders:N0} đơn";
                    lblTotalRevenueVal.Text = $"{quickRes.TotalRevenue:N0} đ";
                    lblAvgOrderVal.Text = $"{quickRes.AverageOrderValue:N0} đ";
                    lblTotalPointsVal.Text = $"{quickRes.TotalPointsAwarded:N0} điểm";
                }

                // 2. Lấy Top sản phẩm bán chạy
                string topQuery = string.IsNullOrEmpty(queryParams) ? "?limit=10" : $"{queryParams}&limit=10";
                var topRes = await client.GetFromJsonAsync<TopProductsApiResultDto>($"reports/top-selling{topQuery}");
                BindTopProductsGrid(topRes?.Data ?? new List<TopSellingProductDto>());

                // 3. Lấy Bảng doanh thu theo khoảng ngày / thời gian
                var revenueRes = await client.GetFromJsonAsync<RevenueByDateApiResultDto>($"reports/revenue-by-date-range{queryParams}");
                bool isSingleDay = (sel == "Hôm nay") || (dtpFrom.Value.Date == dtpTo.Value.Date && sel != "Toàn bộ thời gian");
                BindRevenueGrid(revenueRes?.Data ?? new List<DateRevenueDto>(), isSingleDay);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải báo cáo doanh thu từ máy chủ!\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnViewReport.Enabled = true;
                btnViewReport.Text = "Xem báo cáo";
            }
        }

        private void BindTopProductsGrid(List<TopSellingProductDto> list)
        {
            dgvTopProducts.DataSource = null;
            dgvTopProducts.Columns.Clear();
            dgvTopProducts.AutoGenerateColumns = false;

            dgvTopProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductId",
                HeaderText = "Mã SP",
                Width = 65,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvTopProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "ProductName",
                HeaderText = "Tên sản phẩm",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 9F, FontStyle.Bold) }
            });

            dgvTopProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalQuantitySold",
                HeaderText = "Số lượng",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                }
            });

            dgvTopProducts.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalRevenue",
                HeaderText = "Doanh thu",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(16, 149, 93)
                }
            });

            dgvTopProducts.DataSource = list;
        }

        private void BindRevenueGrid(List<DateRevenueDto> list, bool isHourly)
        {
            dgvRevenue.DataSource = null;
            dgvRevenue.Columns.Clear();
            dgvRevenue.AutoGenerateColumns = false;

            lblRevenueTitle.Text = isHourly ? "BẢNG DOANH THU THEO GIỜ" : "BẢNG DOANH THU THEO NGÀY";

            dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TimePeriod",
                HeaderText = isHourly ? "Khung giờ" : "Ngày bán",
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                DefaultCellStyle = new DataGridViewCellStyle { Font = new Font("Segoe UI", 9F, FontStyle.Bold) }
            });

            dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalOrders",
                HeaderText = "Số đơn",
                Width = 85,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0"
                }
            });

            dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalRevenue",
                HeaderText = "Doanh thu",
                Width = 135,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0",
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(16, 149, 93)
                }
            });

            dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "AverageOrderValue",
                HeaderText = "Hóa đơn TB",
                Width = 115,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0",
                    ForeColor = Color.FromArgb(71, 85, 105)
                }
            });

            dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "TotalPointsAwarded",
                HeaderText = "Điểm tích",
                Width = 85,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N0"
                }
            });

            dgvRevenue.DataSource = list;
        }
    }

    // ===== CÁC LỚP DTO PHỤC VỤ BÁO CÁO =====

    public class QuickReportResultDto
    {
        public string Date { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalPointsAwarded { get; set; }
        public string BestSeller { get; set; } = string.Empty;
        public bool IsSampleData { get; set; }
    }

    public class TopSellingProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class TopProductsApiResultDto
    {
        public bool Success { get; set; }
        public string GeneratedAt { get; set; } = string.Empty;
        public int TotalProducts { get; set; }
        public List<TopSellingProductDto> Data { get; set; } = new List<TopSellingProductDto>();
    }

    public class DateRevenueDto
    {
        public string TimePeriod { get; set; } = string.Empty;
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
        public int TotalPointsAwarded { get; set; }
    }

    public class RevenueByDateApiResultDto
    {
        public bool Success { get; set; }
        public string GeneratedAt { get; set; } = string.Empty;
        public int TotalPeriods { get; set; }
        public decimal GrandTotalRevenue { get; set; }
        public List<DateRevenueDto> Data { get; set; } = new List<DateRevenueDto>();
    }
}
