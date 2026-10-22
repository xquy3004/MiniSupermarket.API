# Hệ Thống Quản Lý Siêu Thị Mini (MiniSupermarket)
> **Đồ án môn học**: Lập trình Ứng dụng .NET Core  
> **Chủ đề**: Cửa hàng Mẹ và Bé (Baby Care Store)  
> **Sinh viên thực hiện**: Nguyễn Xuân Quý  
> **MSSV**: 2124110252 | **Lớp**: CCQ2411C  
> **Nhánh**: `buoi4` (Bài thực hành Buổi 4)

---

## 👥 BÁO CÁO THỰC HÀNH BUỔI 4: PHÂN QUYỀN THEO VAI TRÒ (RBAC) & GIAO DIỆN SHELL WINFORMS

### 1. Mục tiêu buổi học
- Triển khai mô hình phân quyền theo vai trò (Role-Based Access Control - RBAC) chặt chẽ cho 3 bộ phận: **Admin**, **Cashier** (Thu ngân), **Warehouse** (Thủ kho).
- Xây dựng Controller Quản lý sản phẩm (`ProductsController`) và Quản trị người dùng (`UsersController`).
- Xây dựng giao diện chính `FormMainShell` tích hợp thanh Sidebar điều hướng động: tự động ẩn/hiện các nút bấm menu theo quyền hạn thực tế của người đăng nhập.

---

### 2. Ma trận phân quyền hệ thống (RBAC Matrix)

| Chức năng / Endpoint | Admin (Quản trị) | Cashier (Thu ngân) | Warehouse (Thủ kho) |
| :--- | :---: | :---: | :---: |
| **Xem sản phẩm** (`GET /api/products`) | ✅ Cho phép | ✅ Cho phép (để bán hàng) | ✅ Cho phép (để kiểm kho) |
| **Thêm/Sửa/Xóa sản phẩm** (`POST/PUT/DELETE /api/products`) | ✅ Cho phép | ❌ **403 Forbidden** | ✅ **Cho phép** |
| **Tạo và xem hóa đơn** (`/api/orders`) | ✅ Cho phép | ✅ **Cho phép** | ❌ **403 Forbidden** |
| **Quản trị tài khoản nhân viên** (`/api/users`) | ✅ **Toàn quyền** | ❌ **403 Forbidden** | ❌ **403 Forbidden** |
| **Xem báo cáo doanh thu** (`/api/reports`) | ✅ **Cho phép** | ❌ **403 Forbidden** | ❌ **403 Forbidden** |

---

### 3. Chi tiết các Controller mới

#### 3.1. `ProductsController.cs`
- Thao tác thay đổi sản phẩm bọc trong `[Authorize(Roles = "Admin,Warehouse")]`:
  - `POST /api/products`: Thêm sản phẩm mới.
  - `PUT /api/products/{id}`: Sửa giá bán, tồn kho, tên sản phẩm.
  - `DELETE /api/products/{id}`: Xóa sản phẩm (nếu sản phẩm đã có trong hóa đơn thì chặn lại để bảo vệ toàn vẹn dữ liệu).

#### 3.2. `UsersController.cs`
- Toàn bộ Controller bọc trong `[Authorize(Roles = "Admin")]`:
  - `GET /api/users`: Danh sách nhân viên (ẩn mật khẩu băm).
  - `POST /api/users`: Tạo tài khoản mới, phân vai trò Admin/Cashier/Warehouse.
  - `PUT /api/users/{id}/reset-password`: Đặt lại mật khẩu nhân viên.
  - `PUT /api/users/{id}/toggle-lock`: Khóa hoặc mở khóa tài khoản nhân viên (có cơ chế chống Admin tự khóa chính mình).

---

### 4. Giao diện WinForms Shell (`FormMainShell.cs`)
- Thanh Header hiển thị: Tên nhân viên đăng nhập + Vai trò hiện tại.
- Sidebar điều hướng tự động cấu hình quyền hiển thị:
  - **Admin**: Hiển thị đủ **6 nút** (*Bán hàng POS, Quản lý Danh mục, Quản lý Sản phẩm, Quản lý Khách hàng, Báo cáo Doanh thu, Quản trị Tài khoản*).
  - **Cashier**: Chỉ hiển thị **2 nút** (*Bán hàng POS* và *Quản lý Khách hàng*).
  - **Warehouse**: Chỉ hiển thị **2 nút** (*Quản lý Danh mục* và *Quản lý Sản phẩm*).
- Nút **Đăng xuất** an toàn giải phóng Token và đưa về màn hình Đăng nhập.
