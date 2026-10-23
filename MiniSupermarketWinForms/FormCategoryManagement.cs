using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniSupermarketWinForms
{
    public partial class FormCategoryManagement : Form
    {
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lParam);

        private const string ApiBaseAddress = "http://localhost:5000/api/";

        public FormCategoryManagement()
        {
            InitializeComponent();
        }

        private HttpClient GetAuthenticatedClient()
        {
            var client = new HttpClient
            {
                BaseAddress = ApiClientService.Client.BaseAddress ?? new Uri(ApiBaseAddress)
            };

            if (!string.IsNullOrEmpty(SessionManager.JwtToken))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", SessionManager.JwtToken);
            }

            return client;
        }

        // Trạng thái phân trang
        private int _currentPage = 1;
        private int _pageSize = 10;
        private int _totalPages = 1;
        private int _totalItems = 0;
        private string _currentKeyword = string.Empty;

        private async void FormCategoryManagement_Load(object sender, EventArgs e)
        {
            try { SendMessage(txtKeyword.Handle, 0x1501, 1, "Tìm tên danh mục..."); } catch { }
            txtKeyword.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter)
                {
                    btnSearch.PerformClick();
                    ev.Handled = true;
                    ev.SuppressKeyPress = true;
                }
            };
            await LoadPagedDataAsync(1);
        }

        private void FormCategoryManagement_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                PrepareAddNew();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F5)
            {
                _ = LoadPagedDataAsync(1);
                e.Handled = true;
            }
        }

        private async Task LoadPagedDataAsync(int pageNumber = 1)
        {
            if (pageNumber < 1) pageNumber = 1;
            try
            {
                using var client = GetAuthenticatedClient();
                string url = $"categories/paged?pageNumber={pageNumber}&pageSize={_pageSize}";
                if (!string.IsNullOrWhiteSpace(_currentKeyword))
                {
                    url += $"&keyword={Uri.EscapeDataString(_currentKeyword)}";
                }

                var result = await client.GetFromJsonAsync<PagedViewResult<CategoryDto>>(url);
                if (result != null)
                {
                    _currentPage = result.CurrentPage;
                    _totalPages = Math.Max(1, result.TotalPages);
                    _totalItems = result.TotalItems;

                    dgvCategories.DataSource = result.Items;
                    FormatGrid();

                    lblPaging.Text = $"Trang {_currentPage} / {_totalPages}  (Tổng: {_totalItems} DM)";
                    btnFirst.Enabled = _currentPage > 1;
                    btnPrev.Enabled = _currentPage > 1;
                    btnNext.Enabled = _currentPage < _totalPages;
                    btnLast.Enabled = _currentPage < _totalPages;

                    if (dgvCategories.Rows.Count > 0)
                    {
                        SelectRow(0);
                    }
                    else
                    {
                        ClearInputs();
                        lblSelectionInfo.Text = string.IsNullOrWhiteSpace(_currentKeyword)
                            ? "Thông tin danh mục đang chọn: (Chưa có dữ liệu)"
                            : $"Không tìm thấy kết quả với từ khóa '{_currentKeyword}'";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể kết nối đến Server!\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void FormatGrid()
        {
            if (dgvCategories.Columns["CategoryId"] != null)
            {
                dgvCategories.Columns["CategoryId"].HeaderText = "Mã danh mục";
                dgvCategories.Columns["CategoryId"].FillWeight = 20;
            }
            if (dgvCategories.Columns["CategoryName"] != null)
            {
                dgvCategories.Columns["CategoryName"].HeaderText = "Tên danh mục";
                dgvCategories.Columns["CategoryName"].FillWeight = 35;
            }
            if (dgvCategories.Columns["Description"] != null)
            {
                dgvCategories.Columns["Description"].HeaderText = "Mô tả chi tiết";
                dgvCategories.Columns["Description"].FillWeight = 45;
            }
        }

        private void SelectRow(int index)
        {
            if (index >= 0 && index < dgvCategories.Rows.Count)
            {
                dgvCategories.ClearSelection();
                dgvCategories.Rows[index].Selected = true;
                FillDetailsFromRow(dgvCategories.Rows[index]);
            }
        }

        private void FillDetailsFromRow(DataGridViewRow row)
        {
            var id = row.Cells["CategoryId"].Value?.ToString() ?? "";
            var name = row.Cells["CategoryName"].Value?.ToString() ?? "";
            var desc = row.Cells["Description"].Value?.ToString() ?? "";

            txtId.Text = id;
            txtCategoryName.Text = name;
            txtDescription.Text = desc;

            lblSelectionInfo.Text = $"Thông tin danh mục đang chọn ({id})";
        }

        private void dgvCategories_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvCategories.Rows.Count)
            {
                FillDetailsFromRow(dgvCategories.Rows[e.RowIndex]);
            }
        }

        private void PrepareAddNew()
        {
            ClearInputs();
            lblSelectionInfo.Text = "Thông tin danh mục đang chọn: [Đang thêm mới]";
            txtCategoryName.Focus();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            PrepareAddNew();
        }

        private async void btnLoad_Click(object sender, EventArgs e)
        {
            txtKeyword.Clear();
            _currentKeyword = string.Empty;
            await LoadPagedDataAsync(1);
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            _currentKeyword = txtKeyword.Text.Trim();
            await LoadPagedDataAsync(1);
        }

        private async void btnFirst_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                await LoadPagedDataAsync(1);
            }
        }

        private async void btnPrev_Click(object sender, EventArgs e)
        {
            if (_currentPage > 1)
            {
                await LoadPagedDataAsync(_currentPage - 1);
            }
        }

        private async void btnNext_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                await LoadPagedDataAsync(_currentPage + 1);
            }
        }

        private async void btnLast_Click(object sender, EventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                await LoadPagedDataAsync(_totalPages);
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show(
                    "Vui lòng chọn danh mục cần cập nhật!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show(
                    "Tên danh mục không được để trống!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var updateCategory = new
                {
                    CategoryId = id,
                    CategoryName = txtCategoryName.Text.Trim(),
                    Description = txtDescription.Text.Trim()
                };

                using var client = GetAuthenticatedClient();
                var response = await client.PutAsJsonAsync($"categories/{id}", updateCategory);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Cập nhật danh mục thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await LoadPagedDataAsync(_currentPage);
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(
                        "Cập nhật thất bại!\n\n" + message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Có lỗi xảy ra:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtId.Text, out int id))
            {
                MessageBox.Show(
                    "Vui lòng chọn danh mục cần xóa!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa danh mục [{txtCategoryName.Text}] (ID = {id})?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using var client = GetAuthenticatedClient();
                var response = await client.DeleteAsync($"categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(
                        "Xóa danh mục thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    await LoadPagedDataAsync(_currentPage);
                    ClearInputs();
                }
                else
                {
                    string message = await response.Content.ReadAsStringAsync();
                    MessageBox.Show(
                        "Xóa thất bại!\n\n" + message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Có lỗi xảy ra:\n\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
            {
                MessageBox.Show(
                    "Tên danh mục không được để trống!",
                    "Cảnh báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // Nếu đã có ID hợp lệ -> gọi cập nhật; nếu chưa có ID -> gọi thêm mới
            if (int.TryParse(txtId.Text, out int id) && id > 0)
            {
                btnUpdate_Click(sender, e);
            }
            else
            {
                try
                {
                    var newCategory = new
                    {
                        CategoryName = txtCategoryName.Text.Trim(),
                        Description = txtDescription.Text.Trim()
                    };

                    using var client = GetAuthenticatedClient();
                    var response = await client.PostAsJsonAsync("categories", newCategory);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show(
                            "Thêm danh mục mới thành công!",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        await LoadPagedDataAsync(_currentPage);
                    }
                    else
                    {
                        string message = await response.Content.ReadAsStringAsync();
                        MessageBox.Show(
                            "Thêm mới thất bại!\n\n" + message,
                            "Lỗi",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Có lỗi xảy ra:\n\n" + ex.Message,
                        "Lỗi",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void mnuViewDetail_Click(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow != null)
            {
                FillDetailsFromRow(dgvCategories.CurrentRow);
            }
        }

        private void mnuEditCategory_Click(object sender, EventArgs e)
        {
            if (dgvCategories.CurrentRow != null)
            {
                FillDetailsFromRow(dgvCategories.CurrentRow);
                txtCategoryName.Focus();
                txtCategoryName.SelectAll();
            }
        }

        private void mnuDeleteCategory_Click(object sender, EventArgs e)
        {
            btnDelete_Click(sender, e);
        }

        private void ClearInputs()
        {
            txtId.Clear();
            txtCategoryName.Clear();
            txtDescription.Clear();
        }
    }

    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
