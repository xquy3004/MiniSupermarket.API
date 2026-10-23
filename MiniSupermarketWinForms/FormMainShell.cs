using System;
using System.Drawing;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public class FormMainShell : Form
    {
        // ===== Khai báo controls =====
        private Panel panelSidebar;
        private Label lblLogo;
        private FlowLayoutPanel flowMenu;
        private Button btnPOS;
        private Button btnCategory;
        private Button btnProduct;
        private Button btnCustomer;
        private Button btnReports;
        private Button btnUserManage;
        private Button btnRoleManage;
        private Panel panelUserFooter;
        private Button btnLogout;

        private Panel panelTopHeader;
        private Label lblTitle;
        private Label lblUserInfo;

        private Panel panelMainContent;

        // Form con đang được hiển thị trong panelMainContent
        private Form _activeForm = null;

        private readonly Color _menuColor = Color.FromArgb(24, 30, 48);
        private readonly Color _activeColor = Color.FromArgb(41, 100, 180);

        // FormLogin đọc cờ này để biết người dùng bấm "Đăng xuất" hay chỉ đóng cửa sổ
        public bool LoggedOut { get; private set; }

        public FormMainShell()
        {
            InitializeComponent();
            this.Load += FormMainShell_Load;
        }

        // ===== Dựng giao diện bằng code =====
        private void InitializeComponent()
        {
            this.Text = "Hệ thống Quản lý Bán lẻ & Tồn kho Siêu thị Mini";
            this.Size = new Size(1280, 720);
            this.StartPosition = FormStartPosition.CenterScreen;

            // --- Vùng nội dung chính ---
            panelMainContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(244, 245, 247)
            };

            // --- Thanh tiêu đề trên ---
            panelTopHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White
            };
            lblTitle = new Label
            {
                Dock = DockStyle.Fill,
                Text = "BÀN LÀM VIỆC HỆ THỐNG",
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            lblUserInfo = new Label
            {
                Dock = DockStyle.Right,
                Width = 480,
                Text = "Xin chào: ...",
                Font = new Font("Segoe UI", 10F),
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 20, 0)
            };
            panelTopHeader.Controls.Add(lblTitle);
            panelTopHeader.Controls.Add(lblUserInfo);

            // --- Thanh điều hướng trái ---
            panelSidebar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 230,
                BackColor = _menuColor
            };

            lblLogo = new Label
            {
                Dock = DockStyle.Top,
                Height = 70,
                Text = "🛒 MiniMart POS",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            flowMenu = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = _menuColor
            };

            btnPOS = CreateMenuButton("🛒  Bán hàng (POS)", btnPOS_Click);
            btnCategory = CreateMenuButton("📁  Quản lý Danh mục", btnCategory_Click);
            btnProduct = CreateMenuButton("📦  Quản lý Sản phẩm", btnProduct_Click);
            btnCustomer = CreateMenuButton("👥  Quản lý Khách hàng", btnCustomer_Click);
            btnReports = CreateMenuButton("📊  Báo cáo Doanh thu", btnReports_Click);
            btnUserManage = CreateMenuButton("🛡  Quản trị Tài khoản", btnUserManage_Click);
            btnRoleManage = CreateMenuButton("👥  Quản lý Vai trò", btnRoleManage_Click);
            flowMenu.Controls.AddRange(new Control[]
            {
                btnPOS, btnCategory, btnProduct, btnCustomer, btnReports, btnUserManage, btnRoleManage
            });

            panelUserFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = _menuColor
            };
            btnLogout = CreateMenuButton("🚪  Đăng xuất", btnLogout_Click);
            btnLogout.Dock = DockStyle.Fill;
            btnLogout.BackColor = Color.FromArgb(170, 50, 50);
            panelUserFooter.Controls.Add(btnLogout);

            panelSidebar.Controls.Add(flowMenu);
            panelSidebar.Controls.Add(lblLogo);
            panelSidebar.Controls.Add(panelUserFooter);

            // --- Ghép vào Form (thứ tự z-order quyết định cách Dock) ---
            this.Controls.Add(panelMainContent);
            this.Controls.Add(panelTopHeader);
            this.Controls.Add(panelSidebar);
            panelSidebar.SendToBack();       // Sidebar chiếm trọn chiều cao bên trái
            panelMainContent.BringToFront(); // Vùng nội dung lấp phần còn lại
        }

        private Button CreateMenuButton(string text, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(230, 45),
                Margin = new Padding(0),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = _menuColor,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                Font = new Font("Segoe UI", 10.5F),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += onClick;
            return btn;
        }

        // ===== Sự kiện Load =====
        private void FormMainShell_Load(object sender, EventArgs e)
        {
            lblUserInfo.Text = $"Nhân viên: {SessionManager.CurrentUsername} | Vai trò: [{SessionManager.CurrentRole}]";

            // Không hợp lệ thì ApplyRolePermissions đã tự đóng form
            if (!ApplyRolePermissions(SessionManager.CurrentRole)) return;

            OpenDefaultScreenByRole(SessionManager.CurrentRole);
        }

        // ===== Nhúng form con vào panelMainContent =====
        private void OpenChildForm(Form childForm, string screenTitle, Button senderButton)
        {
            if (_activeForm != null)
            {
                _activeForm.Close();
            }

            HighlightActiveButton(senderButton);

            _activeForm = childForm;
            childForm.TopLevel = false;                       // Không còn là cửa sổ độc lập
            childForm.FormBorderStyle = FormBorderStyle.None; // Bỏ viền và thanh tiêu đề
            childForm.Dock = DockStyle.Fill;                  // Tràn toàn bộ vùng nội dung

            panelMainContent.Controls.Clear();
            panelMainContent.Controls.Add(childForm);
            panelMainContent.Tag = childForm;

            lblTitle.Text = screenTitle;
            childForm.BringToFront();
            childForm.Show();
        }

        // Làm nổi bật nút menu đang được chọn
        private void HighlightActiveButton(Button activeButton)
        {
            foreach (Control ctrl in flowMenu.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = _menuColor;
                }
            }
            activeButton.BackColor = _activeColor;
        }

        // ===== Phân quyền hiển thị theo vai trò =====
        // Thứ tự tham số: POS, Danh mục, Sản phẩm, Khách hàng, Báo cáo, Tài khoản
        private void SetMenuVisible(bool pos, bool category, bool product, bool customer, bool reports, bool users, bool roles)
        {
            btnPOS.Visible = pos;
            btnCategory.Visible = category;
            btnProduct.Visible = product;
            btnCustomer.Visible = customer;
            btnReports.Visible = reports;
            btnUserManage.Visible = users;
            btnRoleManage.Visible = roles;
        }

        private bool ApplyRolePermissions(string role)
        {
            switch ((role ?? string.Empty).ToUpper())
            {
                case "ADMIN":
                    SetMenuVisible(true, true, true, true, true, true, true);
                    return true;

                case "CASHIER":
                    SetMenuVisible(true, false, false, true, false, false, false);
                    return true;

                case "WAREHOUSE":
                    SetMenuVisible(false, true, true, false, false, false, false);
                    return true;

                default:
                    MessageBox.Show("Tài khoản chưa được cấp quyền hạn hợp lệ!", "Cảnh báo",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Close();
                    return false;
            }
        }

        // Mở ngay màn hình đúng chuyên môn của từng vai trò
        //private void OpenDefaultScreenByRole(string role)
        //{
        //    switch ((role ?? string.Empty).ToUpper())
        //    {
        //        case "ADMIN":
        //        case "WAREHOUSE":
        //            OpenChildForm(new FormCategoryManagement(), "QUẢN LÝ DANH MỤC NHÓM HÀNG", btnCategory);
        //            break;

        //        case "CASHIER":
        //            OpenChildForm(new FormCustomerManagement(), "QUẢN LÝ KHÁCH HÀNG & TÍCH ĐIỂM", btnCustomer);
        //            break;
        //    }
        //}

        // ===== Sự kiện click các nút trên Sidebar =====
        private void OpenDefaultScreenByRole(string role)
        {
            switch ((role ?? string.Empty).ToUpper())
            {
                case "ADMIN":
                    OpenChildForm(new FormCategoryManagement(), "QUẢN LÝ DANH MỤC NHÓM HÀNG", btnCategory);
                    break;

                case "WAREHOUSE":
                    OpenChildForm(new FormProductManagement(), "QUẢN LÝ THÔNG TIN SẢN PHẨM & KHO HÀNG", btnProduct);
                    break;

                case "CASHIER":
                    OpenChildForm(new FormPOS(), "BÁN HÀNG (POS)", btnPOS);
                    break;
            }
        }
        private void btnCategory_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormCategoryManagement(), "QUẢN LÝ DANH MỤC NHÓM HÀNG", btnCategory);
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormCustomerManagement(), "QUẢN LÝ KHÁCH HÀNG & TÍCH ĐIỂM", btnCustomer);
        }

        // Các màn hình dưới đây sẽ được làm ở các phần B, C, D
        private void btnPOS_Click(object sender, EventArgs e)
        {
            // Lớp bảo vệ thứ 2: Warehouse không có nút POS, nhưng vẫn chặn ở logic
            string role = SessionManager.CurrentRole;
            if (role != "Admin" && role != "Cashier")
            {
                MessageBox.Show("Chỉ Admin và Thu ngân mới được bán hàng!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            OpenChildForm(new FormPOS(), "BÁN HÀNG (POS)", btnPOS);
        }
        private void btnProduct_Click(object sender, EventArgs e)
        {
            OpenChildForm(new FormProductManagement(), "QUẢN LÝ THÔNG TIN SẢN PHẨM & KHO HÀNG", btnProduct);
        }
        private void btnReports_Click(object sender, EventArgs e)
        {
            // Lớp bảo vệ thứ 2: ngoài việc ẩn nút ở Sidebar, chặn luôn ở logic
            if (SessionManager.CurrentRole != "Admin")
            {
                MessageBox.Show("Bạn không có quyền xem dữ liệu tài chính của siêu thị!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            OpenChildForm(new FormQuickReport(), "BÁO CÁO DOANH THU & HIỆU SUẤT CA BÁN", btnReports);
        }
        private void btnUserManage_Click(object sender, EventArgs e)
        {
            // Lớp bảo vệ thứ 2: chỉ Admin được vào màn hình quản trị tài khoản
            if (SessionManager.CurrentRole != "Admin")
            {
                MessageBox.Show("Chỉ quản trị viên mới được quản lý tài khoản!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            OpenChildForm(new FormUserManagement(), "QUẢN TRỊ TÀI KHOẢN VÀ PHÂN QUYỀN HỆ THỐNG", btnUserManage);
        }

        private void btnRoleManage_Click(object sender, EventArgs e)
        {
            if (SessionManager.CurrentRole != "Admin")
            {
                MessageBox.Show("Chỉ quản trị viên mới được quản lý vai trò!", "Từ chối",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            OpenChildForm(new FormRoleManagement(), "QUẢN LÝ VAI TRÒ HỆ THỐNG", btnRoleManage);
        }

        private void ShowComingSoon(string screenName)
        {
            MessageBox.Show($"Màn hình \"{screenName}\" sẽ được làm ở bước tiếp theo.", "Thông báo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất phiên làm việc?", "Xác nhận",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            SessionManager.JwtToken = string.Empty;
            SessionManager.CurrentUsername = string.Empty;
            SessionManager.CurrentRole = string.Empty;

            LoggedOut = true;
            this.Close();
        }
    }
}