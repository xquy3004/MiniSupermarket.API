# Hệ Thống Quản Lý Siêu Thị Mini (MiniSupermarket)
> **Đồ án môn học**: Lập trình Ứng dụng .NET Core  
> **Chủ đề**: Cửa hàng Mẹ và Bé (Baby Care Store)  
> **Sinh viên thực hiện**: Nguyễn Xuân Quý  
> **MSSV**: 2124110252 | **Lớp**: CCQ2411C  
> **Nhánh**: `buoi2` (Bài thực hành Buổi 2)

---

## 🔐 BÁO CÁO THỰC HÀNH BUỔI 2: XÁC THỰC VÀ PHÂN QUYỀN VỚI JWT (JSON WEB TOKEN)

### 1. Mục tiêu buổi học
- Cài đặt cơ chế xác thực phi trạng thái (Stateless Authentication) bằng **JWT Bearer Token**.
- Xây dựng tính năng Đăng nhập bảo mật, băm mật khẩu chuẩn bằng PBKDF2 (`PasswordHelper`).
- Áp dụng phân quyền trên Controller và Action bằng các thuộc tính `[Authorize]` và `[Authorize(Roles = "...")]`.
- Xây dựng giao diện Đăng nhập `FormLogin` trên WinForms, lưu Token vào `SessionManager` và tự động gắn Token vào mỗi Request gửi lên Server.

---

### 2. Các thành phần chính đã xây dựng

#### 2.1. Lớp hỗ trợ băm mật khẩu `PasswordHelper.cs`
- Áp dụng thuật toán `Rfc2898DeriveBytes` kết hợp muối ngẫu nhiên (Salt 16 bytes) và băm SHA-256 (100.000 vòng lặp).
- Hỗ trợ cơ chế kiểm tra linh hoạt cho các tài khoản mẫu kiểm thử:
  ```csharp
  public static bool Verify(string password, string stored)
  ```

#### 2.2. Controller xác thực `AuthController.cs`
- **Endpoint**: `POST /api/auth/login`
- **Quy trình xử lý**:
  1. Kiểm tra tài khoản và mật khẩu thông qua `PasswordHelper.Verify`. Nếu sai $ightarrow$ Trả về `401 Unauthorized`.
  2. Kiểm tra cờ trạng thái `IsActive`. Nếu tài khoản bị khóa $ightarrow$ Trả về `403 Forbidden` (`Tài khoản đã bị khóa!`).
  3. Cấp phát chuỗi JWT Token có thời hạn 2 giờ chứa Claims: `ClaimTypes.Name` (username) và `ClaimTypes.Role` (vai trò).

#### 2.3. Cấu hình bảo vệ API trong `Program.cs`
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
```

#### 2.4. Phân quyền trên `CategoriesController.cs`
- `[Authorize]`: Áp dụng ở cấp Controller (phải đăng nhập mới được xem danh mục).
- `[Authorize(Roles = "Admin")]`: Áp dụng cho `DELETE /api/categories/{id}` và endpoint `GET /api/categories/admin-dashboard`.
- `[Authorize(Roles = "Admin,Cashier")]`: Endpoint `GET /api/categories/staff-pos`.

---

### 3. Giao diện WinForms (`FormLogin` & `SessionManager`)
- **`FormLogin.cs`**:
  - Giao diện đăng nhập tài khoản và mật khẩu.
  - Gọi API `POST /api/auth/login`, nhận JWT Token và vai trò (Role).
- **`SessionManager.cs`**:
  - Lưu trữ phiên làm việc toàn cục: `JwtToken`, `CurrentRole`, `CurrentUsername`.
  - Hàm `AttachToken()` tự động gắn `Authorization: Bearer <Token>` vào `HttpClient.DefaultRequestHeaders`.

---

### 4. Bảng tài khoản kiểm thử
| Tên đăng nhập | Mật khẩu | Vai trò | Trạng thái | Mục đích kiểm thử |
| :--- | :---: | :---: | :---: | :--- |
| `admin` | `123456` | Admin | Đang hoạt động | Kiểm tra quyền Quản trị, truy cập admin-dashboard |
| `cashier01` | `123456` | Cashier | Đang hoạt động | Kiểm tra quyền Thu ngân, bị cấm xóa Category (403) |
| `staff_lock` | `123456` | Cashier | **Đã bị khóa** | Kiểm tra đăng nhập tài khoản bị khóa trả về HTTP 403 |
