using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public class FormPOS : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lParam);

        private const string ApiBaseAddress = "http://localhost:5000/api/";

        // Cấu trúc bố cục chính: Tỷ lệ 7:3 (Bên trái Giỏ hàng 70%, Bên phải Thanh toán 30%)
        private Panel pnlContent;
        private Panel pnlLeftCard;      // Giỏ hàng (70%)
        private Panel pnlCardSpacer;
        private Panel pnlRightCard;     // Thanh toán (30%)

        // ----- Vùng bên trái: GIỎ HÀNG (GIỎ HÀNG) -----
        private Panel pnlLeftTop;
        private Label lblLeftTitle;
        private TextBox txtBarcode;
        private Button btnAddBarcode;
        private Label lblScanStatus;

        private DataGridView dgvCart;
        private Panel pnlLeftBottom;
        private Label lblCartSummary;
        private Button btnDeleteLine;
        private Button btnClearCart;

        // ----- Vùng bên phải: THANH TOÁN (THANH TOÁN) -----
        private Label lblRightTitle;

        // 1. Thẻ thông tin khách hàng
        private GroupBox grpCustomer;
        private TextBox txtCustomerPhone;
        private Button btnFindCustomer;
        private Label lblCustomerInfo;

        // 2. Thẻ tóm tắt tài chính
        private GroupBox grpFinance;
        private Label lblSubTotalTitle;
        private Label lblSubTotal;
        private Label lblDiscountTitle;
        private Label lblDiscount;
        private Label lblTotalTitle;
        private Label lblTotal;

        // 3. Phương thức thanh toán
        private GroupBox grpPaymentMethod;
        private Button btnPayCash;
        private Button btnPayTransfer;
        private Button btnPayCard;

        // 4. Chi tiết tiền mặt (chỉ hiện khi chọn Tiền mặt)
        private Panel pnlCashDetails;
        private Label lblCashReceivedTitle;
        private TextBox txtCashReceived;
        private Label lblChangeTitle;
        private Label lblChange;

        // 5. Nút hoàn tất giao dịch lớn
        private Button btnCheckout;

        // Dữ liệu nội bộ
        private readonly BindingList<CartItem> _cart = new BindingList<CartItem>();
        private List<ProductDto> _allProducts = new List<ProductDto>();
        private CustomerDto _customer = null;
        private string _selectedPaymentMethod = "Tiền mặt";

        public FormPOS()
        {
            InitializeComponent();
            this.Load += FormPOS_Load;
            this.KeyPreview = true;
            this.KeyDown += FormPOS_KeyDown;

            // Kiểm tra phân quyền thu ngân
            string role = SessionManager.CurrentRole;
            bool canSell = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(role, "Cashier", StringComparison.OrdinalIgnoreCase);
            if (!canSell)
            {
                txtBarcode.Enabled = false;
                btnAddBarcode.Enabled = false;
                btnCheckout.Enabled = false;
                SetStatus("Vai trò của bạn không có quyền thu ngân bán hàng.", true);
            }
        }

        private void InitializeComponent()
        {
            this.Text = "BÁN HÀNG (POS)";
            this.ClientSize = new Size(1100, 625);
            this.BackColor = Color.FromArgb(244, 245, 247);
            this.Font = new Font("Segoe UI", 9F);
            this.StartPosition = FormStartPosition.CenterScreen;

            // ==========================================
            // 1. Panel Content bao bọc toàn bộ form
            // ==========================================
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 12, 16, 16),
                BackColor = Color.FromArgb(244, 245, 247)
            };

            // ==========================================
            // 2. Card bên phải: THANH TOÁN (30% - 330px)
            // ==========================================
            pnlRightCard = new Panel
            {
                Dock = DockStyle.Right,
                Width = 330,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12, 10, 12, 10),
                AutoScroll = true
            };

            lblRightTitle = new Label
            {
                Text = "THANH TOÁN ĐƠN HÀNG",
                Location = new Point(12, 8),
                Size = new Size(304, 24),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };

            // 2.1 Thẻ Khách hàng
            grpCustomer = new GroupBox
            {
                Text = "Khách hàng thân thiết (Tích điểm)",
                Location = new Point(10, 36),
                Size = new Size(308, 86),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            txtCustomerPhone = new TextBox
            {
                Location = new Point(10, 24),
                Size = new Size(202, 23),
                Font = new Font("Segoe UI", 9.5F)
            };
            txtCustomerPhone.KeyDown += txtCustomerPhone_KeyDown;
            txtCustomerPhone.TextChanged += txtCustomerPhone_TextChanged;

            btnFindCustomer = new Button
            {
                Text = "TÌM (F2)",
                Location = new Point(218, 23),
                Size = new Size(80, 26),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnFindCustomer.FlatAppearance.BorderSize = 0;
            btnFindCustomer.Click += btnFindCustomer_Click;

            lblCustomerInfo = new Label
            {
                Text = "👤 Khách vãng lai (không tích điểm)",
                Location = new Point(10, 54),
                Size = new Size(288, 24),
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            grpCustomer.Controls.AddRange(new Control[] { txtCustomerPhone, btnFindCustomer, lblCustomerInfo });

            // 2.2 Thẻ Tài chính
            grpFinance = new GroupBox
            {
                Text = "Chi tiết thanh toán",
                Location = new Point(10, 128),
                Size = new Size(308, 104),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            lblSubTotalTitle = new Label { Text = "Tổng tiền hàng:", Location = new Point(12, 22), AutoSize = true, Font = new Font("Segoe UI", 9F) };
            lblSubTotal = new Label
            {
                Text = "0 đ",
                Location = new Point(140, 20),
                Size = new Size(156, 20),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59)
            };

            lblDiscountTitle = new Label { Text = "Giảm giá / Ưu đãi:", Location = new Point(12, 44), AutoSize = true, Font = new Font("Segoe UI", 9F) };
            lblDiscount = new Label
            {
                Text = "0 đ",
                Location = new Point(140, 42),
                Size = new Size(156, 20),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 149, 93)
            };

            lblTotalTitle = new Label
            {
                Text = "TỔNG CỘNG:",
                Location = new Point(12, 70),
                AutoSize = true,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };

            lblTotal = new Label
            {
                Text = "0 đ",
                Location = new Point(110, 64),
                Size = new Size(186, 32),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38)
            };

            grpFinance.Controls.AddRange(new Control[] { lblSubTotalTitle, lblSubTotal, lblDiscountTitle, lblDiscount, lblTotalTitle, lblTotal });

            // 2.3 Phương thức thanh toán
            grpPaymentMethod = new GroupBox
            {
                Text = "Phương thức thanh toán",
                Location = new Point(10, 238),
                Size = new Size(308, 66),
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105)
            };

            btnPayCash = new Button
            {
                Text = "💵 Tiền mặt\n(F8)",
                Location = new Point(8, 20),
                Size = new Size(92, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPayCash.FlatAppearance.BorderSize = 0;
            btnPayCash.Click += (s, e) => SelectPaymentMethod("Tiền mặt");

            btnPayTransfer = new Button
            {
                Text = "💳 C/Khoản\n(F9)",
                Location = new Point(106, 20),
                Size = new Size(96, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPayTransfer.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnPayTransfer.Click += (s, e) => SelectPaymentMethod("Chuyển khoản");

            btnPayCard = new Button
            {
                Text = "📱 Thẻ POS",
                Location = new Point(208, 20),
                Size = new Size(92, 36),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnPayCard.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnPayCard.Click += (s, e) => SelectPaymentMethod("Thẻ POS");

            grpPaymentMethod.Controls.AddRange(new Control[] { btnPayCash, btnPayTransfer, btnPayCard });

            // 2.4 Chi tiết tiền mặt
            pnlCashDetails = new Panel
            {
                Location = new Point(10, 310),
                Size = new Size(308, 66)
            };

            lblCashReceivedTitle = new Label { Text = "Tiền khách đưa:", Location = new Point(2, 6), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
            txtCashReceived = new TextBox
            {
                Location = new Point(110, 2),
                Size = new Size(196, 25),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                TextAlign = HorizontalAlignment.Right
            };
            txtCashReceived.KeyPress += txtCashReceived_KeyPress;
            txtCashReceived.TextChanged += txtCashReceived_TextChanged;

            lblChangeTitle = new Label { Text = "Tiền thừa:", Location = new Point(2, 38), AutoSize = true, Font = new Font("Segoe UI", 9.5F, FontStyle.Bold) };
            lblChange = new Label
            {
                Text = "0 đ",
                Location = new Point(110, 34),
                Size = new Size(196, 24),
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(30, 41, 59)
            };

            pnlCashDetails.Controls.AddRange(new Control[] { lblCashReceivedTitle, txtCashReceived, lblChangeTitle, lblChange });

            // 2.5 Nút thanh toán lớn
            btnCheckout = new Button
            {
                Text = "THANH TOÁN & IN HÓA ĐƠN (F12)",
                Location = new Point(10, 386),
                Size = new Size(308, 46),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(16, 149, 93),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCheckout.FlatAppearance.BorderSize = 0;
            btnCheckout.Click += btnCheckout_Click;

            pnlRightCard.Controls.AddRange(new Control[]
            {
                lblRightTitle, grpCustomer, grpFinance, grpPaymentMethod, pnlCashDetails, btnCheckout
            });

            // ==========================================
            // 3. Khoảng cách giữa 2 Card
            // ==========================================
            pnlCardSpacer = new Panel
            {
                Dock = DockStyle.Right,
                Width = 14
            };

            // ==========================================
            // 4. Card bên trái: GIỎ HÀNG (70%)
            // ==========================================
            pnlLeftCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(14)
            };

            // 4.1 Thanh quét mã & tìm kiếm thông minh trên cùng
            pnlLeftTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                Padding = new Padding(0, 0, 0, 8)
            };

            lblLeftTitle = new Label
            {
                Text = "GIỎ HÀNG",
                Location = new Point(0, 6),
                AutoSize = true,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42)
            };

            txtBarcode = new TextBox
            {
                Location = new Point(90, 4),
                Size = new Size(260, 25),
                Font = new Font("Segoe UI", 9.5F)
            };
            txtBarcode.KeyDown += txtBarcode_KeyDown;

            btnAddBarcode = new Button
            {
                Text = "+ THÊM (Enter)",
                Location = new Point(356, 3),
                Size = new Size(110, 27),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(15, 23, 42),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAddBarcode.FlatAppearance.BorderSize = 0;
            btnAddBarcode.Click += btnAddBarcode_Click;

            lblScanStatus = new Label
            {
                Text = "Sẵn sàng quét mã hoặc gõ tên sản phẩm.",
                Location = new Point(474, 8),
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139)
            };

            pnlLeftTop.Controls.AddRange(new Control[] { lblLeftTitle, txtBarcode, btnAddBarcode, lblScanStatus });

            // 4.2 Bảng giỏ hàng DataGridView
            dgvCart = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(226, 232, 240),
                RowHeadersVisible = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 36,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvCart.RowTemplate.Height = 32;

            dgvCart.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.FromArgb(241, 245, 249),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(6, 0, 6, 0),
                SelectionBackColor = Color.FromArgb(241, 245, 249),
                SelectionForeColor = Color.FromArgb(30, 41, 59)
            };

            dgvCart.DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(4),
                SelectionBackColor = Color.FromArgb(219, 234, 254),
                SelectionForeColor = Color.FromArgb(30, 41, 59)
            };

            // Định nghĩa cột giỏ hàng chuẩn
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colNo",
                HeaderText = "STT",
                ReadOnly = true,
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colBarcode",
                HeaderText = "Mã vạch",
                DataPropertyName = "Barcode",
                ReadOnly = true,
                FillWeight = 22
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "Tên sản phẩm",
                DataPropertyName = "ProductName",
                ReadOnly = true,
                FillWeight = 36
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPrice",
                HeaderText = "Đơn giá (đ)",
                DataPropertyName = "UnitPrice",
                ReadOnly = true,
                FillWeight = 16,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colQty",
                HeaderText = "Số lượng",
                DataPropertyName = "Quantity",
                FillWeight = 14,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLine",
                HeaderText = "Thành tiền (đ)",
                DataPropertyName = "LineTotal",
                ReadOnly = true,
                FillWeight = 18,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            var btnCol = new DataGridViewButtonColumn
            {
                Name = "colAction",
                HeaderText = "Xóa",
                Text = "✕",
                UseColumnTextForButtonValue = true,
                FillWeight = 8,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    ForeColor = Color.FromArgb(220, 38, 38),
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                }
            };
            dgvCart.Columns.Add(btnCol);

            dgvCart.DataSource = _cart;
            dgvCart.CellEndEdit += dgvCart_CellEndEdit;
            dgvCart.CellContentClick += dgvCart_CellContentClick;
            dgvCart.RowPostPaint += dgvCart_RowPostPaint;
            dgvCart.DataError += (s, e) =>
            {
                e.ThrowException = false;
                SetStatus("Số lượng phải là số nguyên dương hợp lệ!", true);
            };

            // 4.3 Vùng chân bảng: Thống kê & nút thao tác nhanh
            pnlLeftBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 38,
                Padding = new Padding(0, 4, 0, 0)
            };

            lblCartSummary = new Label
            {
                Text = "Tổng: 0 mặt hàng (0 sản phẩm)",
                Dock = DockStyle.Left,
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(0, 8)
            };

            var pnlLeftBottomActions = new Panel
            {
                Dock = DockStyle.Right,
                Width = 275,
                Height = 34
            };

            btnDeleteLine = new Button
            {
                Text = "✕ XÓA DÒNG (Del)",
                Location = new Point(0, 1),
                Size = new Size(125, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(220, 38, 38),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnDeleteLine.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnDeleteLine.Click += (s, e) => DeleteSelectedCartRow();

            btnClearCart = new Button
            {
                Text = "🗑 HỦY GIỎ (F4)",
                Location = new Point(132, 1),
                Size = new Size(140, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClearCart.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnClearCart.Click += btnClearCart_Click;

            pnlLeftBottomActions.Controls.AddRange(new Control[] { btnDeleteLine, btnClearCart });
            pnlLeftBottom.Controls.AddRange(new Control[] { pnlLeftBottomActions, lblCartSummary });

            pnlLeftCard.Controls.Add(dgvCart);
            pnlLeftCard.Controls.Add(pnlLeftBottom);
            pnlLeftCard.Controls.Add(pnlLeftTop);

            // ==========================================
            // 5. Ghép vào PanelContent và Form
            // ==========================================
            pnlContent.Controls.Add(pnlLeftCard);
            pnlContent.Controls.Add(pnlCardSpacer);
            pnlContent.Controls.Add(pnlRightCard);

            this.Controls.Add(pnlContent);
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

        private async void FormPOS_Load(object sender, EventArgs e)
        {
            try { SendMessage(txtBarcode.Handle, 0x1501, 1, "Quét mã vạch hoặc gõ tên..."); } catch { }
            try { SendMessage(txtCustomerPhone.Handle, 0x1501, 1, "Nhập SĐT khách..."); } catch { }

            await LoadProductsAsync();
            txtBarcode.Focus();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var products = await client.GetFromJsonAsync<List<ProductDto>>("products");
                _allProducts = products ?? new List<ProductDto>();

                // Cấu hình tự động gợi ý khi gõ tên hoặc mã vạch
                var source = new AutoCompleteStringCollection();
                foreach (var p in _allProducts)
                {
                    if (!string.IsNullOrWhiteSpace(p.Barcode)) source.Add(p.Barcode);
                    if (!string.IsNullOrWhiteSpace(p.ProductName)) source.Add(p.ProductName);
                }
                txtBarcode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                txtBarcode.AutoCompleteSource = AutoCompleteSource.CustomSource;
                txtBarcode.AutoCompleteCustomSource = source;
            }
            catch (Exception ex)
            {
                SetStatus("Lỗi nạp danh mục sản phẩm: " + ex.Message, true);
            }
        }

        // ===== Xử lý thêm hàng vào giỏ =====
        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                ProcessBarcodeOrSearch();
            }
        }

        private void btnAddBarcode_Click(object sender, EventArgs e)
        {
            ProcessBarcodeOrSearch();
        }

        private void ProcessBarcodeOrSearch()
        {
            string input = txtBarcode.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            // 1. Tìm chính xác theo Barcode
            var p = _allProducts.FirstOrDefault(x => string.Equals(x.Barcode, input, StringComparison.OrdinalIgnoreCase));

            // 2. Tìm chính xác theo tên sản phẩm
            if (p == null)
            {
                p = _allProducts.FirstOrDefault(x => string.Equals(x.ProductName, input, StringComparison.OrdinalIgnoreCase));
            }

            // 3. Tìm chứa từ khóa tên sản phẩm
            if (p == null)
            {
                p = _allProducts.FirstOrDefault(x => x.ProductName.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (p != null)
            {
                AddToCart(p, 1);
            }
            else
            {
                _ = SearchBarcodeOnServerAsync(input);
                return;
            }

            txtBarcode.Clear();
            txtBarcode.Focus();
        }

        private async Task SearchBarcodeOnServerAsync(string barcode)
        {
            try
            {
                using var client = GetAuthenticatedClient();
                var res = await client.GetAsync("products/barcode/" + Uri.EscapeDataString(barcode));
                if (res.StatusCode == HttpStatusCode.NotFound)
                {
                    SetStatus($"Không tìm thấy sản phẩm có mã/tên: {barcode}", true);
                }
                else if (res.IsSuccessStatusCode)
                {
                    var p = await res.Content.ReadFromJsonAsync<ProductDto>();
                    if (p != null) AddToCart(p, 1);
                }
                else
                {
                    await ShowFailureAsync(res, "Tra cứu sản phẩm");
                }
            }
            catch (Exception ex)
            {
                SetStatus("Lỗi kết nối: " + ex.Message, true);
            }
            finally
            {
                txtBarcode.Clear();
                txtBarcode.Focus();
            }
        }

        private void AddToCart(ProductDto p, int qty)
        {
            if (p.StockQuantity <= 0)
            {
                SetStatus($"\"{p.ProductName}\" đã hết hàng trong kho!", true);
                return;
            }

            var existing = _cart.FirstOrDefault(c => c.ProductId == p.ProductId);
            if (existing == null)
            {
                _cart.Add(new CartItem
                {
                    ProductId = p.ProductId,
                    Barcode = p.Barcode,
                    ProductName = p.ProductName,
                    UnitPrice = p.Price,
                    Quantity = qty,
                    MaxStock = p.StockQuantity
                });
            }
            else
            {
                existing.MaxStock = p.StockQuantity;
                if (existing.Quantity + qty > existing.MaxStock)
                {
                    SetStatus($"\"{p.ProductName}\" chỉ còn tồn kho {existing.MaxStock} sản phẩm!", true);
                    return;
                }
                existing.Quantity += qty;
            }

            UpdateCartView();
            SetStatus($"Đã thêm: {p.ProductName}", false);
        }

        private void dgvCart_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= _cart.Count) return;
            var item = _cart[e.RowIndex];
            if (item == null) return;

            if (item.Quantity < 1)
            {
                item.Quantity = 1;
                SetStatus("Số lượng tối thiểu là 1!", true);
            }
            else if (item.Quantity > item.MaxStock)
            {
                item.Quantity = item.MaxStock;
                SetStatus($"\"{item.ProductName}\" chỉ còn tồn {item.MaxStock} sản phẩm!", true);
            }

            UpdateCartView();
        }

        private void dgvCart_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dgvCart.Columns["colAction"].Index)
            {
                if (e.RowIndex < _cart.Count)
                {
                    var item = _cart[e.RowIndex];
                    _cart.RemoveAt(e.RowIndex);
                    UpdateCartView();
                    SetStatus($"Đã xóa \"{item.ProductName}\" khỏi giỏ hàng.", false);
                }
            }
        }

        private void dgvCart_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvCart.Rows.Count)
            {
                dgvCart.Rows[e.RowIndex].Cells["colNo"].Value = (e.RowIndex + 1).ToString();
            }
        }

        private void DeleteSelectedCartRow()
        {
            if (dgvCart.CurrentRow != null && dgvCart.CurrentRow.Index >= 0 && dgvCart.CurrentRow.Index < _cart.Count)
            {
                var item = _cart[dgvCart.CurrentRow.Index];
                _cart.RemoveAt(dgvCart.CurrentRow.Index);
                UpdateCartView();
                SetStatus($"Đã xóa \"{item.ProductName}\" khỏi giỏ.", false);
            }
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0) return;

            var confirm = MessageBox.Show("Xác nhận hủy toàn bộ giỏ hàng hiện tại?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            ResetOrderInputs();
            txtBarcode.Focus();
        }

        private void ResetOrderInputs()
        {
            _cart.Clear();
            txtCashReceived.Clear();
            txtCustomerPhone.Clear();
            ResetCustomer();
            UpdateCartView();
        }

        private void UpdateCartView()
        {
            int totalItems = _cart.Count;
            int totalQty = _cart.Sum(c => c.Quantity);
            decimal total = _cart.Sum(c => c.LineTotal);

            lblCartSummary.Text = $"Tổng: {totalItems} mặt hàng ({totalQty} món)";
            lblSubTotal.Text = total.ToString("N0") + " đ";
            lblTotal.Text = total.ToString("N0") + " đ";
            CalculateChange();
            dgvCart.Refresh();
        }

        // ===== Xử lý khách hàng thân thiết =====
        private void txtCustomerPhone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                btnFindCustomer.PerformClick();
            }
        }

        private void txtCustomerPhone_TextChanged(object sender, EventArgs e)
        {
            if (_customer != null && txtCustomerPhone.Text.Trim() != _customer.PhoneNumber)
            {
                ResetCustomer();
            }
        }

        private async void btnFindCustomer_Click(object sender, EventArgs e)
        {
            await FindCustomerAsync();
        }

        private async Task<bool> FindCustomerAsync()
        {
            string phone = txtCustomerPhone.Text.Trim();
            if (phone.Length == 0)
            {
                ResetCustomer();
                return true;
            }

            try
            {
                using var client = GetAuthenticatedClient();
                var res = await client.GetAsync("customers/search?keyword=" + Uri.EscapeDataString(phone));
                if (!res.IsSuccessStatusCode)
                {
                    await ShowFailureAsync(res, "Tìm khách hàng");
                    return false;
                }

                var list = await res.Content.ReadFromJsonAsync<List<CustomerDto>>();
                var match = list?.FirstOrDefault(c => c.PhoneNumber == phone);

                if (match == null)
                {
                    _customer = null;
                    lblCustomerInfo.Text = "⚠️ Không tìm thấy SĐT này (tính khách vãng lai)";
                    lblCustomerInfo.ForeColor = Color.FromArgb(220, 38, 38);
                    return true;
                }

                _customer = match;
                lblCustomerInfo.Text = $"👤 {match.CustomerName} - Hạng {match.MembershipRank} ({match.RewardPoints} điểm)";
                lblCustomerInfo.ForeColor = Color.FromArgb(16, 149, 93);
                return true;
            }
            catch (Exception ex)
            {
                SetStatus("Lỗi kết nối khách hàng: " + ex.Message, true);
                return false;
            }
        }

        private void ResetCustomer()
        {
            _customer = null;
            lblCustomerInfo.Text = "👤 Khách vãng lai (không tích điểm)";
            lblCustomerInfo.ForeColor = Color.FromArgb(100, 116, 139);
        }

        // ===== Xử lý phương thức thanh toán & Tiền mặt =====
        private void SelectPaymentMethod(string method)
        {
            _selectedPaymentMethod = method;
            bool isCash = method == "Tiền mặt";

            pnlCashDetails.Visible = isCash;
            btnCheckout.Location = new Point(10, isCash ? 386 : 316);

            HighlightPayButton(btnPayCash, isCash);
            HighlightPayButton(btnPayTransfer, method == "Chuyển khoản");
            HighlightPayButton(btnPayCard, method == "Thẻ POS");

            if (isCash)
            {
                txtCashReceived.Focus();
                txtCashReceived.SelectAll();
            }
            else
            {
                txtCashReceived.Text = _cart.Sum(c => c.LineTotal).ToString("N0");
            }
        }

        private void HighlightPayButton(Button btn, bool active)
        {
            if (active)
            {
                btn.BackColor = Color.FromArgb(30, 41, 59);
                btn.ForeColor = Color.White;
            }
            else
            {
                btn.BackColor = Color.FromArgb(241, 245, 249);
                btn.ForeColor = Color.FromArgb(30, 41, 59);
            }
        }

        private void txtCashReceived_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCashReceived_TextChanged(object sender, EventArgs e)
        {
            CalculateChange();
        }

        private bool TryGetCash(out decimal cash)
        {
            string raw = txtCashReceived.Text.Replace(",", "").Replace(".", "").Trim();
            return decimal.TryParse(raw, out cash);
        }

        private void CalculateChange()
        {
            decimal total = _cart.Sum(c => c.LineTotal);

            if (TryGetCash(out decimal cash))
            {
                decimal change = cash - total;
                if (change >= 0)
                {
                    lblChange.Text = change.ToString("N0") + " đ";
                    lblChange.ForeColor = Color.FromArgb(16, 149, 93);
                }
                else
                {
                    lblChange.Text = "Chưa đủ tiền!";
                    lblChange.ForeColor = Color.FromArgb(220, 38, 38);
                }
            }
            else
            {
                lblChange.Text = "0 đ";
                lblChange.ForeColor = Color.FromArgb(30, 41, 59);
            }
        }

        // ===== Phím tắt toàn màn hình theo tài liệu thiết kế =====
        private void FormPOS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F8)
            {
                SelectPaymentMethod("Tiền mặt");
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F9)
            {
                SelectPaymentMethod("Chuyển khoản");
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F12)
            {
                if (btnCheckout.Enabled) btnCheckout.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F4)
            {
                btnClearCart.PerformClick();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F2)
            {
                txtCustomerPhone.Focus();
                txtCustomerPhone.SelectAll();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F1 || e.KeyCode == Keys.F3)
            {
                txtBarcode.Focus();
                txtBarcode.SelectAll();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete && dgvCart.Focused)
            {
                DeleteSelectedCartRow();
                e.Handled = true;
            }
        }

        // ===== Thanh toán =====
        private async void btnCheckout_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống! Vui lòng quét hoặc thêm sản phẩm trước.", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBarcode.Focus();
                return;
            }

            dgvCart.EndEdit();
            decimal total = _cart.Sum(c => c.LineTotal);

            decimal? cashReceived = null;
            if (_selectedPaymentMethod == "Tiền mặt")
            {
                if (txtCashReceived.Text.Trim().Length > 0)
                {
                    if (!TryGetCash(out decimal cash))
                    {
                        MessageBox.Show("Số tiền khách đưa không hợp lệ!", "Cảnh báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCashReceived.Focus();
                        return;
                    }
                    if (cash < total)
                    {
                        MessageBox.Show("Tiền khách đưa chưa đủ để thanh toán đơn hàng!", "Cảnh báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtCashReceived.Focus();
                        return;
                    }
                    cashReceived = cash;
                }
            }
            else
            {
                cashReceived = total;
            }

            btnCheckout.Enabled = false;
            try
            {
                if (_customer == null && txtCustomerPhone.Text.Trim().Length > 0)
                {
                    bool ok = await FindCustomerAsync();
                    if (!ok) return;

                    if (_customer == null)
                    {
                        var goOn = MessageBox.Show(
                            "Không tìm thấy thông tin khách hàng với SĐT này.\nBạn có muốn tiếp tục bán cho khách vãng lai (không tích điểm)?",
                            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (goOn != DialogResult.Yes) return;
                    }
                }

                var confirm = MessageBox.Show(
                    string.Format("Xác nhận thanh toán đơn hàng:\n- Tổng số món: {0}\n- Tổng tiền: {1:N0} đ\n- Hình thức: {2}?",
                        _cart.Sum(c => c.Quantity), total, _selectedPaymentMethod),
                    "Xác nhận thanh toán", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm != DialogResult.Yes) return;

                var payload = new
                {
                    CustomerId = _customer != null ? (int?)_customer.CustomerId : null,
                    Items = _cart.Select(c => new { c.ProductId, c.Quantity }).ToList()
                };

                using var client = GetAuthenticatedClient();
                var res = await client.PostAsJsonAsync("orders", payload);

                if (res.IsSuccessStatusCode)
                {
                    var order = await res.Content.ReadFromJsonAsync<OrderResultDto>();
                    ShowReceipt(order, cashReceived);

                    ResetOrderInputs();
                    SetStatus("Thanh toán thành công. Sẵn sàng cho đơn hàng tiếp theo.", false);

                    await LoadProductsAsync();
                }
                else
                {
                    await ShowFailureAsync(res, "Thanh toán đơn hàng");
                    await LoadProductsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra trong quá trình thanh toán:\n\n" + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCheckout.Enabled = true;
                txtBarcode.Focus();
            }
        }

        private void ShowReceipt(OrderResultDto order, decimal? cashReceived)
        {
            if (order == null) return;

            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine("    SIÊU THỊ MẸ & BÉ BABY CARE STORE");
            sb.AppendLine("            HÓA ĐƠN BÁN LẺ");
            sb.AppendLine("========================================");
            sb.AppendLine(string.Format("Mã đơn: #{0}", order.OrderId));
            sb.AppendLine(string.Format("Thời gian: {0:dd/MM/yyyy HH:mm:ss}", order.OrderDate));
            sb.AppendLine(string.Format("Thu ngân: {0}", order.CashierUsername));
            sb.AppendLine(string.Format("Khách hàng: {0}", order.CustomerName));
            sb.AppendLine(string.Format("Hình thức: {0}", _selectedPaymentMethod));
            sb.AppendLine(new string('-', 40));
            sb.AppendLine(string.Format("{0,-20} {1,4} {2,12}", "Sản phẩm", "SL", "Thành tiền"));
            sb.AppendLine(new string('-', 40));
            foreach (var it in order.Items)
            {
                string name = it.ProductName.Length > 20 ? it.ProductName.Substring(0, 18) + ".." : it.ProductName;
                sb.AppendLine(string.Format("{0,-20} {1,4} {2,12:N0} đ", name, it.Quantity, it.LineTotal));
            }
            sb.AppendLine(new string('-', 40));
            sb.AppendLine(string.Format("TỔNG CỘNG: {0:N0} đ", order.TotalAmount));
            if (cashReceived.HasValue && _selectedPaymentMethod == "Tiền mặt")
            {
                sb.AppendLine(string.Format("Tiền khách đưa: {0:N0} đ", cashReceived.Value));
                sb.AppendLine(string.Format("Tiền thừa trả khách: {0:N0} đ", (cashReceived.Value - order.TotalAmount)));
            }
            if (order.PointsEarned > 0)
            {
                sb.AppendLine(string.Format("Điểm tích lũy nhận được: +{0} điểm", order.PointsEarned));
            }
            sb.AppendLine("========================================");
            sb.AppendLine("  CẢM ƠN QUÝ KHÁCH & HẸN GẶP LẠI!");

            MessageBox.Show(sb.ToString(), "HÓA ĐƠN BÁN HÀNG",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SetStatus(string text, bool isError)
        {
            lblScanStatus.Text = text;
            lblScanStatus.ForeColor = isError ? Color.FromArgb(220, 38, 38) : Color.FromArgb(16, 149, 93);
        }

        private async Task ShowFailureAsync(HttpResponseMessage res, string action)
        {
            if (res.StatusCode == HttpStatusCode.Forbidden)
            {
                MessageBox.Show(string.Format("{0} thất bại: Bạn không có quyền thực hiện thao tác này!", action), "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (res.StatusCode == HttpStatusCode.Unauthorized)
            {
                MessageBox.Show("Phiên làm việc đã hết hạn. Vui lòng đăng xuất và đăng nhập lại!", "Hết phiên",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string body = await res.Content.ReadAsStringAsync();
            string message = body;
            try
            {
                using (var doc = JsonDocument.Parse(body))
                {
                    if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                        doc.RootElement.TryGetProperty("message", out var m))
                    {
                        message = m.GetString() ?? body;
                    }
                }
            }
            catch (JsonException) { }

            MessageBox.Show(string.Format("{0} thất bại!\n\n{1}", action, message), "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxStock { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

    public class OrderResultDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string CashierUsername { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int PointsEarned { get; set; }
        public List<OrderItemResultDto> Items { get; set; } = new List<OrderItemResultDto>();
    }

    public class OrderItemResultDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal LineTotal { get; set; }
    }
}
