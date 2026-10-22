# Hệ Thống Quản Lý Siêu Thị Mini (MiniSupermarket)
> **Đồ án môn học**: Lập trình Ứng dụng .NET Core  
> **Chủ đề**: Cửa hàng Mẹ và Bé (Baby Care Store)  
> **Sinh viên thực hiện**: Nguyễn Xuân Quý  
> **Mã số sinh viên (MSSV)**: 2124110252  
> **Lớp**: CCQ2411C  

---

## 📌 1. Giới thiệu tổng quan
Dự án **MiniSupermarket** được xây dựng theo kiến trúc phân tầng chuẩn mực **Client - Server**:
- **Backend (Web API)**: Xây dựng trên nền tảng **ASP.NET Core Web API (.NET 8)**, sử dụng **Entity Framework Core**, **ADO.NET**, **SQL Server LocalDB**, bảo mật xác thực phân quyền bằng **JWT Bearer Token** và bảo vệ giao dịch bán hàng qua **SQL Transaction (ACID)**.
- **Frontend (Client)**: Ứng dụng **Windows Forms (.NET Desktop)** giao tiếp với Backend qua giao thức HTTP RESTful API sử dụng `HttpClient` dùng chung và `System.Net.Http.Json`.
- **Cơ sở dữ liệu**: Thiết kế theo chủ đề thực tế **Cửa hàng Mẹ và Bé (Baby Care Store)** gồm 6 thực thể nghiệp vụ: Tài khoản người dùng (`Users`), Nhóm hàng (`Categories`), Sản phẩm (`Products`), Khách hàng thân thiết (`Customers`), Hóa đơn (`Orders`) và Chi tiết hóa đơn (`OrderDetails`).

---

## 👥 2. Danh sách tài khoản kiểm thử (Testing Accounts)
Hệ thống phân quyền theo 3 vai trò chính với mật khẩu kiểm thử dùng chung: **`123456`**.

| Tài khoản (Username) | Mật khẩu | Họ và tên nhân viên | Vai trò (Role) | Chức năng & Quyền hạn |
| :--- | :---: | :--- | :---: | :--- |
| **`admin`** | `123456` | Nguyễn Thị Quản Trị | **Admin** | Toàn quyền hệ thống: POS, Danh mục, Sản phẩm, Khách hàng, Báo cáo doanh thu, Quản trị tài khoản. |
| **`cashier01`** | `123456` | Lê Thị Thu Ngân 1 | **Cashier** | Thu ngân quầy POS: Quét mã vạch bán hàng, tìm kiếm & tích điểm khách hàng thân thiết. Không truy cập kho & báo cáo. |
| **`warehouse01`** | `123456` | Đặng Văn Thủ Kho 1 | **Warehouse** | Quản lý kho: Xem & chỉnh sửa nhóm hàng, quản lý nhập xuất tồn sản phẩm. Không được phép bán hàng và xem báo cáo. |
| **`staff_lock`** | `123456` | Hồ Thị Nhân Viên Nghỉ | **Cashier** | Tài khoản bị khóa (`IsActive = false`) phục vụ kiểm thử chặn đăng nhập (HTTP 403). |

---

## 🚀 3. Tiến độ hoàn thành qua từng Buổi học (Buổi 1 – 6 & Phụ lục)

### 🔹 Buổi 1: Thiết kế RESTful API CRUD Nhóm hàng (Categories)
- Thiết kế Model `Category` và xây dựng `CategoriesController`.
- Cung cấp đầy đủ các endpoint chuẩn RESTful: `GET` danh sách, `GET/{id}`, `GET/search?keyword=`, `POST` tạo mới (validate dữ liệu rỗng $\rightarrow$ 400), `PUT` cập nhật $\rightarrow$ 204, `DELETE` xóa $\rightarrow$ 204.
- Tích hợp Swagger / OpenAPI UI tương tác trực tiếp trên trình duyệt.

### 🔹 Buổi 2: Xác thực JWT Bearer & Phân quyền bảo mật
- Triển khai `AuthController` với hàm sinh token `GenerateJwtToken` sử dụng thuật ngữ ký bảo mật `HmacSha256Signature`.
- Cơ chế băm và xác thực mật khẩu an toàn với thuật toán chuẩn PBKDF2 (`PasswordHelper`).
- Kiểm tra tài khoản bị khóa (`IsActive == false`) trả về `403 Forbidden`.
- Bảo vệ các endpoint nhạy cảm bằng `[Authorize]` và `[Authorize(Roles = "Admin")]`.
- Tạo màn hình `FormLogin` trên WinForms lưu phiên làm việc `SessionManager`.

### 🔹 Buổi 3: Tích hợp EF Core, SQL Server & Quản lý Khách hàng
- Kết nối CSDL SQL Server LocalDB qua `SupermarketDbContext` và `AddDbContext`.
- Quản lý khách hàng thân thiết (`CustomersController`): CRUD, tìm kiếm theo tên và số điện thoại.
- Thiết lập ràng buộc quan hệ nâng cao: Khi xóa khách hàng cũ có hóa đơn liên kết, hóa đơn vẫn được bảo toàn nguyên vẹn và trường `CustomerId` tự động gán về `NULL` (`OnDelete: SetNull`).

### 🔹 Buổi 4: Phân quyền theo vai trò (RBAC) & Giao diện WinForms Shell
- Thiết lập ma trận phân quyền chặt chẽ trên API:
  - `Products`: Chỉ Admin và Warehouse được quyền Thêm/Sửa/Xóa; Cashier bị chặn (403).
  - `Orders`: Chỉ Admin và Cashier được lập hóa đơn; Warehouse bị chặn (403).
  - `Users`: Chỉ Admin được quản lý tài khoản nhân viên.
  - `Reports`: Bảo vệ phân quyền chỉ dành riêng cho Admin.
- Xây dựng giao diện điều hướng trung tâm `FormMainShell` với thanh Sidebar tự động ẩn/hiện nút chức năng theo đúng vai trò đăng nhập.

### 🔹 Buổi 5: ADO.NET, Stored Procedures & Giao dịch POS (ACID Transaction)
- Thao tác ADO.NET Connected Mode (`SqlDataReader`) và Disconnected Mode (`DataTable` & `SqlDataAdapter`).
- Tích hợp 3 Stored Procedures phục vụ báo cáo tài chính:
  1. `sp_GetRevenueByCashier`: Báo cáo doanh thu theo từng thu ngân.
  2. `sp_GetRevenueByDateRange`: Báo cáo doanh thu theo khoảng ngày có tham số.
  3. `sp_GetTop5SellingProducts`: Báo cáo top 5 sản phẩm bán chạy nhất.
- Xây dựng `CheckoutController`: Xử lý giao dịch thanh toán POS bọc trong `SqlTransaction` chuẩn ACID (Commit khi đủ tồn kho, Rollback an toàn khi vượt tồn kho, không để lại dữ liệu rác).
- Tính năng Hủy đơn hàng (`DELETE /api/checkout/cancel/{orderId}`): Hoàn trả số lượng vào kho, khấu trừ lại điểm thưởng khách hàng và xóa sạch hóa đơn.

### 🔹 Buổi 6: Phân trang LINQ (Eager Loading) & Tích hợp Client - Server
- Phân trang LINQ IQueryable kết hợp nạp dữ liệu liên quan Eager Loading (`.Include(p => p.Category)`).
- `GET /api/products/paged`: Tìm kiếm theo từ khóa kết hợp lọc danh mục `categoryId`, tự động tính toán `TotalPages`.
- Xử lý biên phòng thủ an toàn: Tự chuẩn hóa `pageSize = 10` khi truyền số âm hoặc 0; trả về mảng rỗng `[]` khi trang vượt quá tổng.
- `GET /api/customers/paged`: Sắp xếp giảm dần theo điểm tích lũy (`RewardPoints`).
- Hoàn thiện giao diện WinForms: Tích hợp thanh điều khiển phân trang "Trang trước / Trang sau", ComboBox lọc danh mục và màn hình Bán hàng POS.

### 🔹 Phụ lục: Khởi tạo 6 thực thể & Data Seeding Cửa hàng Mẹ và Bé
- Cấu hình trọn vẹn 6 bảng dữ liệu qua EF Core ModelBuilder Data Seeding:
  - 15 nhóm hàng Mẹ và Bé (`Categories`).
  - 50 sản phẩm Mẹ và Bé (`Products`) với mã vạch chuẩn định dạng `MB00...`
  - 15 khách hàng thân thiết (`Customers`) với điểm thưởng và hạng thành viên.
  - 15 tài khoản nhân viên (`Users`) thuộc 3 vai trò.
  - 15 hóa đơn bán lẻ (`Orders`).
  - 23 dòng chi tiết hóa đơn (`OrderDetails`).
- Đạt 100% yêu cầu mỗi bảng có tối thiểu 15 bản ghi thực tế.

---

## 🧪 4. Kết quả kiểm thử tự động (Automation Test Results)
Hệ thống đã trải qua quá trình kiểm thử tự động 54 test case toàn diện:

| Phân hệ / Buổi | Số lượng Test | Kết quả | Trạng thái |
| :--- | :---: | :---: | :---: |
| **Phụ lục** (CSDL Seeding & Model) | 2 | 2/2 | ✅ **100% ĐẠT** |
| **Buổi 1** (CRUD Nhóm hàng) | 8 | 8/8 | ✅ **100% ĐẠT** |
| **Buổi 2** (JWT & Xác thực) | 7 | 7/7 | ✅ **100% ĐẠT** |
| **Buổi 3** (EF Core & Khách hàng) | 12 | 12/12 | ✅ **100% ĐẠT** |
| **Buổi 4** (Phân quyền RBAC) | 5 | 5/5 | ✅ **100% ĐẠT** |
| **Buổi 5** (ADO.NET, SP & Transaction) | 14 | 14/14 | ✅ **100% ĐẠT** |
| **Buổi 6** (Phân trang LINQ & Eager Loading) | 6 | 6/6 | ✅ **100% ĐẠT** |
| **TỔNG CỘNG** | **54** | **54/54** | 🏆 **100% PASS** |

---

## 💻 5. Hướng dẫn cài đặt & Khởi chạy

### 5.1. Chuẩn bị môi trường
- .NET 8.0 SDK.
- SQL Server (khuyến nghị **SQL Server LocalDB** đi kèm Visual Studio).
- Visual Studio 2022 (với Workload *.NET Desktop Development* và *ASP.NET and web development*).

### 5.2. Cấu hình Cơ sở dữ liệu
1. Mở file `MiniSupermarket.API/appsettings.json` và kiểm tra chuỗi kết nối:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MiniSupermarketDb;Trusted_Connection=True;TrustServerCertificate=True;"
   }
   ```
2. Cập nhật Database bằng lệnh Migration:
   ```bash
   dotnet ef database update --project MiniSupermarket.API
   ```
3. Mở SQL Server Management Studio (SSMS) kết nối tới `(localdb)\MSSQLLocalDB`, mở và thực thi file script để tạo 3 Stored Procedures:
   ```text
   MiniSupermarket.API/Scripts/Buoi5_StoredProcedures.sql
   ```

### 5.3. Khởi chạy ứng dụng
**Cách 1: Khởi chạy bằng Visual Studio (Khuyến nghị)**
- Mở file solution `MiniSupermarket.API.sln`.
- Nhấp chuột phải vào Solution $\rightarrow$ chọn **Configure Startup Projects...**
- Chọn **Multiple startup projects**:
  - `MiniSupermarket.API`: **Start**
  - `MiniSupermarketWinForms`: **Start**
- Nhấn **F5** để khởi chạy đồng thời cả Backend Web API và giao diện Client WinForms.

**Cách 2: Khởi chạy bằng dòng lệnh (CLI)**
```bash
# Terminal 1: Khởi động Backend Web API (chạy tại cổng 5000)
dotnet run --project MiniSupermarket.API --urls "http://localhost:5000"

# Terminal 2: Khởi động Frontend WinForms
dotnet run --project MiniSupermarketWinForms
```
- Truy cập giao diện tài liệu Swagger: `http://localhost:5000/swagger`
