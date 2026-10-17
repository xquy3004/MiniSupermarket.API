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
        private const string ApiBaseAddress = "http://localhost:5000/api/";

        // ----- Bên trái: quét mã vạch + danh sách hàng -----
        private TextBox txtBarcode;
        private Button btnAddBarcode;
        private Label lblScanStatus;
        private TextBox txtFilter;
        private DataGridView dgvProducts;

        // ----- Bên phải: khách hàng + giỏ hàng + thanh toán -----
        private TextBox txtCustomerPhone;
        private Button btnFindCustomer;
        private Label lblCustomerName;
        private DataGridView dgvCart;
        private Label lblTotal;
        private TextBox txtCashReceived;
        private Label lblChange;
        private Button btnClearCart;
        private Button btnCheckout;

        private readonly BindingList<CartItem> _cart = new BindingList<CartItem>();
        private List<ProductDto> _allProducts = new List<ProductDto>();

        // Khách hàng thân thiết đang được chọn (null = khách vãng lai)
        private CustomerDto _customer = null;

        public FormPOS()
        {
            InitializeComponent();
            this.Load += FormPOS_Load;
            this.KeyPreview = true;
            this.KeyDown += FormPOS_KeyDown;

            // Lớp bảo vệ phía client: server cũng chặn (403) với vai trò khác
            string role = SessionManager.CurrentRole;
            bool canSell = string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(role, "Cashier", StringComparison.OrdinalIgnoreCase);
            if (!canSell)
            {
                txtBarcode.Enabled = false;
                btnAddBarcode.Enabled = false;
                btnCheckout.Enabled = false;
                SetStatus("Vai trò của bạn không được phép bán hàng.", true);
            }
        }

        // ===== Dựng giao diện bằng code =====
        private void InitializeComponent()
        {
            this.Text = "Bán hàng (POS)";
            this.Size = new Size(1100, 640);
            this.BackColor = Color.FromArgb(244, 245, 247);
            this.Font = new Font("Segoe UI", 10F);

            // ================= Bên phải: khách hàng + giỏ hàng + thanh toán =================
            var panelRight = new Panel { Dock = DockStyle.Right, Width = 480, BackColor = Color.White };

            // --- Khách hàng thân thiết ---
            var panelCustomer = new Panel { Dock = DockStyle.Top, Height = 112 };
            var lblPhone = new Label
            {
                Text = "SĐT khách hàng thân thiết (để tích điểm):",
                AutoSize = true,
                Location = new Point(12, 8)
            };
            txtCustomerPhone = new TextBox { Location = new Point(12, 32), Width = 310 };
            txtCustomerPhone.KeyDown += txtCustomerPhone_KeyDown;
            txtCustomerPhone.TextChanged += txtCustomerPhone_TextChanged;
            btnFindCustomer = CreateButton("Tìm khách", Color.FromArgb(41, 100, 180), 332, 30, 130, 30);
            btnFindCustomer.Click += btnFindCustomer_Click;
            lblCustomerName = new Label
            {
                Text = "Khách vãng lai",
                AutoSize = false,
                Location = new Point(12, 70),
                Size = new Size(450, 34),
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(90, 90, 90)
            };
            panelCustomer.Controls.AddRange(new Control[] { lblPhone, txtCustomerPhone, btnFindCustomer, lblCustomerName });

            // --- Khu thanh toán (dưới cùng) ---
            var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 255 };

            var lblTotalCaption = new Label
            {
                Text = "TỔNG CỘNG:",
                AutoSize = true,
                Location = new Point(12, 18),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold)
            };
            lblTotal = new Label
            {
                Text = "0 đ",
                AutoSize = false,
                Location = new Point(150, 8),
                Size = new Size(312, 40),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(192, 57, 43),
                Font = new Font("Segoe UI", 20F, FontStyle.Bold)
            };

            var lblCashCaption = new Label { Text = "Tiền khách đưa:", AutoSize = true, Location = new Point(12, 64) };
            txtCashReceived = new TextBox
            {
                Location = new Point(190, 58),
                Width = 272,
                TextAlign = HorizontalAlignment.Right,
                Font = new Font("Segoe UI", 13F)
            };
            txtCashReceived.KeyPress += txtCashReceived_KeyPress;
            txtCashReceived.TextChanged += txtCashReceived_TextChanged;

            var lblChangeCaption = new Label { Text = "Tiền thừa trả khách:", AutoSize = true, Location = new Point(12, 104) };
            lblChange = new Label
            {
                Text = "0 đ",
                AutoSize = false,
                Location = new Point(190, 98),
                Size = new Size(272, 32),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold)
            };

            btnClearCart = CreateButton("HỦY GIỎ HÀNG", Color.FromArgb(192, 57, 43), 12, 140, 450, 36);
            btnCheckout = CreateButton("THANH TOÁN (F9)", Color.FromArgb(30, 132, 73), 12, 188, 450, 48);
            btnCheckout.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnClearCart.Click += btnClearCart_Click;
            btnCheckout.Click += btnCheckout_Click;

            panelBottom.Controls.AddRange(new Control[]
            {
                lblTotalCaption, lblTotal, lblCashCaption, txtCashReceived,
                lblChangeCaption, lblChange, btnClearCart, btnCheckout
            });

            // --- Bảng giỏ hàng ---
            dgvCart = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colName",
                HeaderText = "Sản phẩm",
                DataPropertyName = "ProductName",
                ReadOnly = true,
                FillWeight = 140
            });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colQty",
                HeaderText = "SL",
                DataPropertyName = "Quantity",
                FillWeight = 40
            });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPrice",
                HeaderText = "Đơn giá",
                DataPropertyName = "UnitPrice",
                ReadOnly = true,
                FillWeight = 70,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCart.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLine",
                HeaderText = "Thành tiền",
                DataPropertyName = "LineTotal",
                ReadOnly = true,
                FillWeight = 80,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0", Alignment = DataGridViewContentAlignment.MiddleRight }
            });
            dgvCart.DataSource = _cart;
            dgvCart.CellEndEdit += dgvCart_CellEndEdit;
            dgvCart.DataError += (s, e) =>
            {
                // Gõ chữ vào ô số lượng: bỏ qua, ô tự quay về giá trị cũ
                e.ThrowException = false;
                SetStatus("Số lượng phải là số nguyên!", true);
            };

            panelRight.Controls.Add(dgvCart);
            panelRight.Controls.Add(panelCustomer);
            panelRight.Controls.Add(panelBottom);
            dgvCart.BringToFront();

            // ================= Bên trái: quét mã + danh sách hàng =================
            var panelLeft = new Panel { Dock = DockStyle.Fill };

            var panelScan = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.White };
            var lblScan = new Label
            {
                Text = "Quét hoặc nhập mã vạch, rồi nhấn Enter:",
                AutoSize = true,
                Location = new Point(15, 12)
            };
            txtBarcode = new TextBox
            {
                Location = new Point(15, 38),
                Width = 300,
                Font = new Font("Segoe UI", 13F)
            };
            txtBarcode.KeyDown += txtBarcode_KeyDown;
            btnAddBarcode = CreateButton("Thêm vào giỏ", Color.FromArgb(41, 100, 180), 330, 36, 140, 36);
            btnAddBarcode.Click += btnAddBarcode_Click;
            lblScanStatus = new Label
            {
                Text = "Sẵn sàng.",
                AutoSize = false,
                Location = new Point(15, 80),
                Size = new Size(520, 28),
                ForeColor = Color.FromArgb(90, 90, 90)
            };
            panelScan.Controls.AddRange(new Control[] { lblScan, txtBarcode, btnAddBarcode, lblScanStatus });

            var panelFilter = new Panel { Dock = DockStyle.Top, Height = 46 };
            var lblFilter = new Label { Text = "Tìm trong danh sách:", AutoSize = true, Location = new Point(15, 13) };
            txtFilter = new TextBox { Location = new Point(165, 10), Width = 260 };
            txtFilter.TextChanged += (s, e) => ApplyProductFilter();
            var lblHint = new Label
            {
                Text = "(nhấp đúp vào dòng để thêm vào giỏ)",
                AutoSize = true,
                ForeColor = Color.Gray,
                Location = new Point(440, 13)
            };
            panelFilter.Controls.AddRange(new Control[] { lblFilter, txtFilter, lblHint });

            dgvProducts = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false
            };
            dgvProducts.CellDoubleClick += dgvProducts_CellDoubleClick;

            panelLeft.Controls.Add(dgvProducts);
            panelLeft.Controls.Add(panelFilter);
            panelLeft.Controls.Add(panelScan);
            dgvProducts.BringToFront();

            // ================= Ghép vào Form =================
            this.Controls.Add(panelLeft);
            this.Controls.Add(panelRight);
            panelLeft.BringToFront();
        }

        private Button CreateButton(string text, Color color, int x, int y, int w, int h)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ===== Gọi API =====
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
                ApplyProductFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp sản phẩm: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Lọc danh sách hàng ngay trên máy (không gọi lại API)
        private void ApplyProductFilter()
        {
            string key = (txtFilter.Text ?? string.Empty).Trim();
            IEnumerable<ProductDto> view = _allProducts;

            if (key.Length > 0)
            {
                view = view.Where(p =>
                    p.ProductName.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    p.Barcode.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            dgvProducts.DataSource = view.ToList();

            SetHeader("ProductId", "Mã SP");
            SetHeader("Barcode", "Mã vạch");
            SetHeader("ProductName", "Tên sản phẩm");
            SetHeader("Price", "Đơn giá");
            SetHeader("StockQuantity", "Tồn kho");
            SetHeader("CategoryName", "Nhóm hàng");
            if (dgvProducts.Columns["CategoryId"] != null) dgvProducts.Columns["CategoryId"].Visible = false;
            if (dgvProducts.Columns["Price"] != null) dgvProducts.Columns["Price"].DefaultCellStyle.Format = "N0";
        }

        private void SetHeader(string columnName, string text)
        {
            var col = dgvProducts.Columns[columnName];
            if (col != null) col.HeaderText = text;
        }

        // ===== Khách hàng thân thiết (tra theo SĐT) =====
        private void txtCustomerPhone_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnFindCustomer.PerformClick();
            }
        }

        // Sửa SĐT sau khi đã tìm thấy khách thì bỏ chọn khách cũ, tránh tích điểm nhầm người
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

        // Trả về true nếu tra cứu chạy được (kể cả khi không thấy khách), false nếu lỗi
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
                var match = list == null ? null : list.FirstOrDefault(c => c.PhoneNumber == phone);

                if (match == null)
                {
                    _customer = null;
                    lblCustomerName.Text = "Không tìm thấy SĐT này (tính là khách vãng lai)";
                    lblCustomerName.ForeColor = Color.FromArgb(192, 57, 43);
                    return true;
                }

                _customer = match;
                lblCustomerName.Text = $"{match.CustomerName} - Hạng {match.MembershipRank} ({match.RewardPoints} điểm)";
                lblCustomerName.ForeColor = Color.FromArgb(39, 130, 70);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void ResetCustomer()
        {
            _customer = null;
            lblCustomerName.Text = "Khách vãng lai";
            lblCustomerName.ForeColor = Color.FromArgb(90, 90, 90);
        }

        // ===== Thêm hàng vào giỏ =====
        private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // chặn tiếng "bíp" của Windows
                btnAddBarcode.PerformClick();
            }
        }

        private async void btnAddBarcode_Click(object sender, EventArgs e)
        {
            string barcode = txtBarcode.Text.Trim();
            if (barcode.Length == 0) return;

            try
            {
                using var client = GetAuthenticatedClient();
                var res = await client.GetAsync("products/barcode/" + Uri.EscapeDataString(barcode));

                if (res.StatusCode == HttpStatusCode.NotFound)
                {
                    SetStatus($"Không tìm thấy mã vạch: {barcode}", true);
                }
                else if (res.IsSuccessStatusCode)
                {
                    var p = await res.Content.ReadFromJsonAsync<ProductDto>();
                    if (p != null) AddToCart(p, 1);
                }
                else
                {
                    await ShowFailureAsync(res, "Tra cứu mã vạch");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            txtBarcode.Clear();
            txtBarcode.Focus();
        }

        private void dgvProducts_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var p = dgvProducts.Rows[e.RowIndex].DataBoundItem as ProductDto;
            if (p != null) AddToCart(p, 1);
            txtBarcode.Focus();
        }

        private void AddToCart(ProductDto p, int qty)
        {
            if (p.StockQuantity <= 0)
            {
                SetStatus($"\"{p.ProductName}\" đã hết hàng!", true);
                return;
            }

            var existing = _cart.FirstOrDefault(c => c.ProductId == p.ProductId);
            if (existing == null)
            {
                _cart.Add(new CartItem
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    UnitPrice = p.Price,
                    Quantity = qty,
                    MaxStock = p.StockQuantity
                });
            }
            else
            {
                existing.MaxStock = p.StockQuantity; // lấy tồn kho mới nhất
                if (existing.Quantity + qty > existing.MaxStock)
                {
                    SetStatus($"\"{p.ProductName}\" chỉ còn {existing.MaxStock} sản phẩm!", true);
                    return;
                }
                existing.Quantity += qty;
            }

            dgvCart.Refresh();
            UpdateTotal();
            SetStatus($"Đã thêm: {p.ProductName}", false);
        }

        // ===== Sửa số lượng trực tiếp trên bảng giỏ hàng =====
        private void dgvCart_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var item = dgvCart.Rows[e.RowIndex].DataBoundItem as CartItem;
            if (item == null) return;

            if (item.Quantity < 1)
            {
                item.Quantity = 1;
                SetStatus("Số lượng tối thiểu là 1 (muốn bỏ hết hàng thì bấm \"HỦY GIỎ HÀNG\").", true);
            }
            else if (item.Quantity > item.MaxStock)
            {
                item.Quantity = item.MaxStock;
                SetStatus($"\"{item.ProductName}\" chỉ còn {item.MaxStock} sản phẩm!", true);
            }

            dgvCart.Refresh();
            UpdateTotal();
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0) return;

            var confirm = MessageBox.Show("Hủy toàn bộ giỏ hàng hiện tại?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            ResetOrderInputs();
            txtBarcode.Focus();
        }

        // Dọn giỏ và các ô nhập của đơn hiện tại
        private void ResetOrderInputs()
        {
            _cart.Clear();
            txtCashReceived.Clear();
            txtCustomerPhone.Clear(); // TextChanged sẽ tự đưa khách về "Khách vãng lai"
            ResetCustomer();
            UpdateTotal();
        }

        // ===== Tính tổng, tiền thừa =====
        private void UpdateTotal()
        {
            decimal total = _cart.Sum(c => c.LineTotal);
            lblTotal.Text = total.ToString("N0") + " đ";
            CalculateChange();
        }

        // Chỉ cho gõ chữ số vào ô tiền khách đưa
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
            return decimal.TryParse(txtCashReceived.Text.Trim(), out cash);
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
                    lblChange.ForeColor = Color.Black;
                }
                else
                {
                    lblChange.Text = "Chưa đủ tiền!";
                    lblChange.ForeColor = Color.Red;
                }
            }
            else
            {
                lblChange.Text = "0 đ";
                lblChange.ForeColor = Color.Black;
            }
        }

        // ===== Thanh toán =====
        private void FormPOS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9 && btnCheckout.Enabled)
            {
                e.SuppressKeyPress = true;
                btnCheckout.PerformClick();
            }
        }

        private async void btnCheckout_Click(object sender, EventArgs e)
        {
            if (_cart.Count == 0)
            {
                MessageBox.Show("Giỏ hàng đang trống!", "Cảnh báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kết thúc việc sửa ô đang dở (nếu có) để số lượng được ghi nhận
            dgvCart.EndEdit();

            decimal total = _cart.Sum(c => c.LineTotal);

            // Kiểm tra tiền khách đưa (để trống thì bỏ qua, coi như thanh toán đủ)
            decimal? cashReceived = null;
            if (txtCashReceived.Text.Trim().Length > 0)
            {
                if (!TryGetCash(out decimal cash))
                {
                    MessageBox.Show("Tiền khách đưa không hợp lệ!", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cash < total)
                {
                    MessageBox.Show("Khách đưa chưa đủ tiền!", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                cashReceived = cash;
            }

            btnCheckout.Enabled = false; // chống bấm đúp gửi hai lần
            try
            {
                // Đã gõ SĐT nhưng chưa bấm "Tìm khách": tự tra giúp thu ngân
                if (_customer == null && txtCustomerPhone.Text.Trim().Length > 0)
                {
                    bool ok = await FindCustomerAsync();
                    if (!ok) return;

                    if (_customer == null)
                    {
                        var goOn = MessageBox.Show(
                            "Không tìm thấy khách có SĐT này.\nTiếp tục bán cho khách vãng lai (không tích điểm)?",
                            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (goOn != DialogResult.Yes) return;
                    }
                }

                var confirm = MessageBox.Show(
                    $"Thanh toán {_cart.Count} mặt hàng, tổng {total:N0} đ?", "Xác nhận thanh toán",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
                    SetStatus("Thanh toán thành công. Sẵn sàng đơn tiếp theo.", false);

                    // Tồn kho đã đổi trên server: nạp lại cho đúng
                    await LoadProductsAsync();
                }
                else
                {
                    await ShowFailureAsync(res, "Thanh toán");
                    // Có thể do hết hàng: nạp lại tồn kho để thu ngân thấy số mới
                    await LoadProductsAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra:\n\n" + ex.Message, "Lỗi",
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
            sb.AppendLine($"HÓA ĐƠN #{order.OrderId}");
            sb.AppendLine($"Thời gian: {order.OrderDate:dd/MM/yyyy HH:mm}");
            sb.AppendLine($"Thu ngân: {order.CashierUsername}");
            sb.AppendLine($"Khách hàng: {order.CustomerName}");
            sb.AppendLine(new string('-', 40));
            foreach (var it in order.Items)
            {
                sb.AppendLine($"{it.ProductName}");
                sb.AppendLine($"    {it.Quantity} x {it.UnitPrice:N0} = {it.LineTotal:N0} đ");
            }
            sb.AppendLine(new string('-', 40));
            sb.AppendLine($"TỔNG CỘNG: {order.TotalAmount:N0} đ");
            if (cashReceived.HasValue)
            {
                sb.AppendLine($"Tiền khách đưa: {cashReceived.Value:N0} đ");
                sb.AppendLine($"Tiền thừa: {(cashReceived.Value - order.TotalAmount):N0} đ");
            }
            if (order.PointsEarned > 0)
            {
                sb.AppendLine($"Điểm tích lũy được: +{order.PointsEarned}");
            }

            MessageBox.Show(sb.ToString(), "Thanh toán thành công",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ===== Tiện ích =====
        private void SetStatus(string text, bool isError)
        {
            lblScanStatus.Text = text;
            lblScanStatus.ForeColor = isError ? Color.FromArgb(192, 57, 43) : Color.FromArgb(39, 130, 70);
        }

        // Server trả lỗi dạng {"message":"..."}: lấy riêng phần message cho dễ đọc
        private async Task ShowFailureAsync(HttpResponseMessage res, string action)
        {
            if (res.StatusCode == HttpStatusCode.Forbidden)
            {
                MessageBox.Show($"{action} thất bại: bạn không có quyền thực hiện thao tác này!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            if (res.StatusCode == HttpStatusCode.Unauthorized)
            {
                MessageBox.Show("Phiên làm việc đã hết hạn, vui lòng đăng xuất và đăng nhập lại!", "Hết phiên",
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
            catch (JsonException)
            {
                // Không phải JSON: hiển thị nguyên văn
            }

            MessageBox.Show($"{action} thất bại!\n\n{message}", "Lỗi",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Một dòng trong giỏ hàng
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public int MaxStock { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;
    }

    // Kết quả hóa đơn do server trả về sau khi thanh toán
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