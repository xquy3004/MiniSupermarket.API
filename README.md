Hệ Thống Quản Lý Siêu Thị Mini (Mini Supermarket System)
Sinh viên: Nguyễn Xuân Quý — MSSV: 2124110252 Lớp: CCQ2411C

Đồ án môn: Lập trình Ứng dụng .NET Core

1. Giới thiệu
Xây dựng hệ thống Quản lý Bán lẻ & Tồn kho Siêu thị Mini theo mô hình phân tầng Client - Server chuẩn mực:

Backend (Web API): xử lý nghiệp vụ, quản lý dữ liệu.
Frontend (WinForms): giao diện người dùng, giao tiếp với Backend qua HttpClient.
Ứng dụng thao tác trực tiếp với dữ liệu In-Memory (RAM) để tập trung vào tư duy thiết kế RESTful API và xử lý bất đồng bộ async/await, tạo nền tảng vững chắc để bước sang các buổi tiếp theo (tích hợp SQL Server + Entity Framework Core + JWT).

2. Kiến trúc Solution
Lau_Market
├── API/                     # Backend: ASP.NET Core Web API (.NET 8)
│   ├── Controllers/
│   │   ├── CategoriesController.cs   # CRUD + Tìm kiếm nhóm hàng
│   │   └── RolesController.cs        # CRUD vai trò (bài tập mở rộng)
│   ├── Models/
│   │   ├── Category.cs               # Nhóm hàng hóa
│   │   └── Role.cs                   # Vai trò nhân viên
│   ├── Program.cs                    # Cấu hình Swagger, Controllers
│   └── Properties/launchSettings.json
└── FE/                      # Frontend: Windows Forms (.NET 8)
    ├── FormCategoryManagement.*      # Quản lý nhóm hàng (CRUD + Tìm kiếm)
    ├── FormRoleManagement.*          # Quản lý vai trò (bài tập mở rộng)
    └── Program.cs                    # Điểm khởi chạy
3. Backend (Web API)
3.1. Model Category — Nhóm hàng hóa
Thuộc tính	Kiểu	Mô tả
CategoryId	int	Mã định danh (Khóa chính)
CategoryName	string	Tên nhóm hàng (bắt buộc)
Description	string?	Mô tả chi tiết (có thể trống)
3.2. Model Role — Vai trò nhân viên
Thuộc tính	Kiểu	Mô tả
Id	int	Mã định danh vai trò (Khóa chính)
RoleName	string	Tên vai trò (Ví dụ: Admin, Cashier, Warehouse)
Description	string?	Mô tả chức năng của vai trò
3.3. Các Endpoint
Categories — /api/categories

Phương thức	Đường dẫn	Chức năng
GET	/api/categories	Lấy toàn bộ danh sách
GET	/api/categories/{id}	Lấy chi tiết theo ID
GET	/api/categories/search?keyword=...	Tìm kiếm theo từ khóa
POST	/api/categories	Thêm mới
PUT	/api/categories/{id}	Cập nhật
DELETE	/api/categories/{id}	Xóa
Roles — /api/roles

Phương thức	Đường dẫn	Chức năng
GET	/api/roles	Lấy toàn bộ danh sách
GET	/api/roles/{id}	Lấy chi tiết theo ID
POST	/api/roles	Thêm mới
PUT	/api/roles/{id}	Cập nhật
DELETE	/api/roles/{id}	Xóa
3.4. Swagger UI
Backend được cấu hình Swagger/OpenAPI trong Program.cs (gói Swashbuckle.AspNetCore) ở môi trường Development:

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
...
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
Truy cập: https://localhost:7065/swagger

4. Frontend (WinForms)
4.1. FormCategoryManagement — Quản lý nhóm hàng
DataGridView dgvCategories: hiển thị danh sách (3 cột CategoryId, CategoryName, Description).
TextBox: txtId, txtCategoryName, txtDescription, txtKeyword.
Nút lệnh: btnLoad, btnAdd, btnUpdate, btnDelete, btnSearch.
Kết nối HttpClient cố định đến https://localhost:7065/api/.
Dùng gói System.Net.Http.Json với các hàm tiện ích: GetFromJsonAsync, PostAsJsonAsync, PutAsJsonAsync, DeleteAsync.
CategoryDto (Client) trung gian hứng dữ liệu JSON từ Server.
4.2. FormRoleManagement — Quản lý vai trò (Bài tập mở rộng)
DataGridView dgvRoles: hiển thị danh sách (3 cột Id, RoleName, Description).
TextBox: txtId, txtRoleName, txtDescription.
Nút lệnh: btnLoad, btnAdd, btnUpdate, btnDelete.
Tương tác qua HttpClient tới /api/roles.
Mở trực tiếp từ nút "Quản lý vai trò" trên màn hình quản lý nhóm hàng.
5. Hướng dẫn chạy
Yêu cầu: .NET 8 SDK, Visual Studio 2022 (hoặc dotnet CLI). Chứng chỉ HTTPS phát triển đã tin cậy (dotnet dev-certs https --trust).

Cách 1: Visual Studio (khuyến nghị)
Mở Solution Lau_Market.sln.
Đặt 2 project cùng khởi chạy: API + FE (Startup Project).
Nhấn F5.
API tự mở trang Swagger; WinForms mở màn hình quản lý nhóm hàng.
Cách 2: dòng lệnh
# Terminal 1 - chạy Backend (HTTPS)
dotnet run --project API --launch-profile https

# Terminal 2 - chạy Frontend
dotnet run --project FE
Trình tự kiểm thử
Trên Swagger, kiểm tra nhóm Categories và Roles (GET/POST/PUT/DELETE).
Mở form WinForms → bảng hiển thị dữ liệu từ Server.
Thực hiện đầy đủ Thêm, Sửa, Xóa, Tìm kiếm trên giao diện và quan sát đồng bộ dữ liệu qua HttpClient.
6. Ghi chú
Dữ liệu hiện lưu In-Memory (static List), sẽ mất khi khởi động lại API — phục vụ mục đích học tập.
Port mặc định của Backend: HTTPS 7065, HTTP 5207. Nếu thay đổi port, cần cập nhật BaseAddress ở 2 form trong project FE.
Các bước tiếp theo của đồ án: tích hợp SQL Server + Entity Framework Core, bảo mật phân quyền nâng cao (JWT).