# Hệ Thống Quản Lý Siêu Thị Mini (MiniSupermarket)
> **Đồ án môn học**: Lập trình Ứng dụng .NET Core  
> **Chủ đề**: Siêu thị Mini Mẹ & Bé (Baby Care Store)  
> **Sinh viên thực hiện**: Nguyễn Xuân Quý  
> **MSSV**: 2124110252 | **Lớp**: CCQ2411C  
> **Repository GitHub**: [xquy3004/MiniSupermarket.API](https://github.com/xquy3004/MiniSupermarket.API)  
> **Nhánh chính**: `master` (Phiên bản nghiệm thu đồ án hoàn chỉnh)

---

## 📖 1. TỔNG QUAN HỆ THỐNG
Hệ thống phần mềm quản lý bán hàng siêu thị mini chuyên ngành Mẹ & Bé (Baby Care Store) được xây dựng theo kiến trúc phân tầng hiện đại:
- **Tầng Backend**: ASP.NET Core 8.0 Web API cung cấp các chuẩn RESTful API, xác thực & phân quyền bảo mật qua JWT Bearer Token, kiểm soát toàn vẹn giao dịch thanh toán bằng `SqlTransaction` (chuẩn ACID), tối ưu tra cứu nhanh bằng ADO.NET thuần (`SqlDataReader`) và quản lý dữ liệu đối tượng qua Entity Framework Core (Code-First & Migrations).
- **Tầng Frontend**: Windows Forms (.NET Framework 4.8) thiết kế theo mô hình Master-Detail Shell hiện đại, gọi API tập trung qua `HttpClient`, phân quyền giao diện động theo vai trò (Role-based UI) và tích hợp quầy tính tiền POS thông minh.
- **Cơ sở dữ liệu**: Microsoft SQL Server LocalDB gồm 6 thực thể: `Users`, `Categories`, `Products`, `Customers`, `Orders`, `OrderDetails`.

---

## 🚀 2. HƯỚNG DẪN KHỞI CHẠY ĐỒ ÁN (HOW TO RUN)

### Cách 1: Khởi chạy trực tiếp từ Visual Studio 2022 (Khuyến nghị khi chấm bài)
1. Mở tệp Solution: `MiniSupermarket.API.sln`.
2. Thiết lập khởi chạy đồng thời cả Backend và WinForms:
   - Nhấp chuột phải vào Solution `'MiniSupermarket.API'` trong cửa sổ **Solution Explorer** $\rightarrow$ chọn **Properties** (hoặc **Configure Startup Projects...**).
   - Chọn mục **Multiple startup projects**:
     - `MiniSupermarket.API` $\rightarrow$ Action: **Start**
     - `MiniSupermarketWinForms` $\rightarrow$ Action: **Start**
   - Bấm **Apply** $\rightarrow$ **OK**.
3. Bấm phím **F5** (hoặc nút **Start** màu xanh).
   - Backend sẽ chạy tại cổng `http://localhost:5000` và tự động mở trang Swagger UI.
   - Frontend WinForms sẽ khởi chạy màn hình Đăng nhập `FormLogin`.

### Cách 2: Khởi chạy nhanh bằng file `.exe` đã biên dịch sẵn
1. **Bật Backend Web API**:
   - Truy cập: `MiniSupermarket.API\bin\Debug\net8.0\`
   - Nhấp đúp chạy file: `MiniSupermarket.API.exe` (Cửa sổ Kestrel đen sẽ mở và lắng nghe tại `http://localhost:5000`).
2. **Bật Frontend WinForms**:
   - Truy cập: `MiniSupermarketWinForms\bin\Debug\`
   - Nhấp đúp chạy file: `MiniSupermarketWinForms.exe`.
   - Giao diện đăng nhập hiện đại Baby Care Store sẽ hiển thị ngay lập tức.

### Cách 3: Khởi chạy Backend bằng .NET CLI
```bash
# Di chuyển vào thư mục Web API và chạy
dotnet run --project MiniSupermarket.API\MiniSupermarket.API.csproj --launch-profile http
```

---

## 👥 3. DANH SÁCH 15 TÀI KHOẢN KIỂM THỬ HỆ THỐNG
Tất cả tài khoản mẫu được nạp tự động qua Migration theo đúng chuẩn yêu cầu đề bài Buổi 4.  
**Mật khẩu dùng chung cho tất cả tài khoản**: `123456` *(Được băm mật mã học chuẩn PBKDF2)*.

| STT | Tài khoản (Username) | Họ và tên nhân viên | Vai trò (Role) | Phạm vi quyền hạn trên giao diện & API |
| :---: | :--- | :--- | :---: | :--- |
| **1** | `admin01` | Nguyễn Quản Trị | **Admin** | Toàn quyền: Xem báo cáo, quản lý tài khoản, danh mục, sản phẩm, POS. |
| **2** | `admin02` | Trần Giám Đốc | **Admin** | Toàn quyền: Quản trị cấp cao và xem thống kê doanh thu. |
| **3** | `admin_backup` | Đặng Hỗ Trợ | **Admin** | Toàn quyền: Quản trị viên dự phòng hệ thống. |
| **4** | `supervisor` | Mai Giám Sát | **Admin** | Toàn quyền: Giám sát toàn bộ hoạt động quầy thu ngân và tồn kho. |
| **5** | `cashier01` | Lê Thu Ngân | **Cashier** | Thu ngân ca sáng: Chỉ truy cập màn hình POS bán hàng & Khách hàng. |
| **6** | `cashier02` | Phạm Bán Hàng | **Cashier** | Thu ngân: Bán hàng quầy POS, tích lũy điểm thưởng thành viên. |
| **7** | `cashier03` | Hoàng Thu Ngân | **Cashier** | Thu ngân: Bán hàng quầy POS, tra cứu mã vạch và tính tiền thối. |
| **8** | `cashier04` | Vũ Thị Quầy | **Cashier** | Thu ngân: Bán hàng quầy POS, lập hóa đơn thanh toán ACID. |
| **9** | `cashier05` | Đỗ Bán Lẻ | **Cashier** | Thu ngân: Bán hàng POS, tra cứu thông tin khách hàng thân thiết. |
| **10** | `cashier06` | Hồ Ca Chiều | **Cashier** | Thu ngân ca chiều: Bán hàng POS và kiểm tra tích điểm. |
| **11** | `ware01` | Ngô Quản Kho | **Warehouse** | Quản lý kho: Chỉ truy cập màn hình Danh mục & Quản lý Sản phẩm. |
| **12** | `ware02` | Bùi Kiểm Kê | **Warehouse** | Nhân viên kho: Tra cứu tồn kho, cập nhật số lượng và nhóm hàng. |
| **13** | `ware03` | Dương Thủ Kho | **Warehouse** | Thủ kho: Quản lý nhập xuất, theo dõi cảnh báo hàng sắp hết. |
| **14** | `ware04` | Lý Nhập Hàng | **Warehouse** | Quản lý kho: Cập nhật thông tin chi tiết sản phẩm và giá bán. |
| **15** | `ware05` | Trương Vận Chuyển | **Warehouse** | Quản lý kho: Kiểm soát hàng hóa xuất kho và danh mục Mẹ & Bé. |

---

## 🏷️ 4. BỘ MÃ VẠCH MẪU ĐỂ TEST QUÉT MÃ TẠI QUẦY POS
| Mã vạch (Barcode) | Tên mặt hàng (Chuyên ngành Mẹ & Bé) | Nhóm hàng | Đơn giá | Tồn kho mồi |
| :---: | :--- | :--- | :---: | :---: |
| `MB001001` | Sữa bột NAN Optipro 1 (0-6 tháng) hộp 800g | Sữa bột & Sữa công thức | 520.000 đ | 45 |
| `MB001002` | Sữa bột Enfamil A+ số 2 (6-12 tháng) hộp 900g | Sữa bột & Sữa công thức | 490.000 đ | 50 |
| `MB001003` | Sữa bột Aptamil số 3 (1-3 tuổi) hộp 800g | Sữa bột & Sữa công thức | 480.000 đ | 40 |
| `MB002001` | Bỉm dán Pampers Premium Protection NB 76 miếng | Bỉm, Tã & Khăn ướt | 310.000 đ | 85 |
| `MB002002` | Tã quần Huggies Platinum Nature Made size M 64 miếng | Bỉm, Tã & Khăn ướt | 375.000 đ | 70 |
| `MB003001` | Bột ăn dặm Nestlé Cerelac Gạo & Sữa lúa mì 200g | Đồ ăn dặm & Bột ăn dặm | 78.000 đ | 90 |
| `MB004001` | Bình sữa Pigeon PPSU nội địa Nhật 240ml | Bình sữa & Núm ty | 385.000 đ | 55 |
| `MB005001` | Sữa tắm gội 2 trong 1 Chicco hoa cúc 500ml | Tắm gội & Chăm sóc da bé | 275.000 đ | 60 |

---

## 📊 5. TỔNG HỢP KẾT QUẢ KIỂM THỬ TỰ ĐỘNG (54/54 TEST CASES PASS)
| Phân hệ / Tiến độ | Số lượng Test | Kết quả thực tế | Trạng thái kỹ thuật |
| :--- | :---: | :---: | :---: |
| **Phụ lục** (CSDL Seeding & Model Mẹ và Bé) | 2 | 2/2 | ✅ **100% ĐẠT** |
| **Buổi 1** (CRUD Nhóm hàng - Categories) | 8 | 8/8 | ✅ **100% ĐẠT** |
| **Buổi 2** (JWT Authentication & Authorization) | 7 | 7/7 | ✅ **100% ĐẠT** |
| **Buổi 3** (EF Core, Customers & Ràng buộc SetNull) | 12 | 12/12 | ✅ **100% ĐẠT** |
| **Buổi 4** (Phân quyền theo vai trò RBAC) | 5 | 5/5 | ✅ **100% ĐẠT** |
| **Buổi 5** (ADO.NET, SP & Transaction ACID) | 14 | 14/14 | ✅ **100% ĐẠT** |
| **Buổi 6** (Phân trang LINQ & Eager Loading) | 6 | 6/6 | ✅ **100% ĐẠT** |
| **TỔNG CỘNG** | **54** | **54/54** | 🏆 **100% PASS** |

---

## 🛠️ 6. CẤU TRÚC REPOSITORY & LỊCH SỬ COMMIT THEO TIẾN ĐỘ
- **Nhánh `master`**: Nhánh tổng hợp toàn diện, tích hợp giao diện WinForms hiện đại và toàn bộ các tính năng hoàn thiện.
- **Nhánh `fix-final`**: Nhánh đồng bộ chi tiết các sửa đổi cấu trúc Backend và bảo mật.
- **Các nhánh tiến độ thực hành theo buổi**:
  - `buoi2`: Quản lý danh mục và đăng nhập JWT.
  - `buoi3`: Quản lý khách hàng, EF Core, ràng buộc khóa ngoại.
  - `buoi4`: Khung điều khiển FormMainShell, phân quyền 3 vai trò và 15 tài khoản.
  - `buoi5`: Quầy bán hàng POS, giao dịch SqlTransaction, Stored Procedures.
  - `buoi6`: Quản lý sản phẩm phân trang LINQ Server-side và Eager Loading Include.
