# BÁO CÁO ĐÁNH GIÁ VÀ CHẤM ĐIỂM ĐỒ ÁN MÔN HỌC

> **Đề tài**: *Nghiên cứu Kiến trúc RESTful Web API trong Xây dựng Phần mềm Quản lý Bán hàng - Siêu thị Mini Mẹ & Bé Baby Care Store*  
> **Môn học**: Lập trình Ứng dụng .NET Core (ASP.NET Core Web API + EF Core + SQL Server + JWT + WinForms)  
> **Sinh viên thực hiện**: Nguyễn Xuân Quý — MSSV: 2124110252 — Lớp: CCQ2411C  
> **Môi trường thẩm định**: Base URL `http://localhost:5000` (ASP.NET Core 8.0 Kestrel) | SQL Server LocalDB `(localdb)\MSSQLLocalDB` (Database: `MiniSupermarketDb`)  
> **Phương pháp kiểm định**: Chạy API/SQL thật bằng mã kiểm thử tự động, đọc trực tiếp mã nguồn C#, lược đồ CSDL và lịch sử Git. **Tuyệt đối không sửa mã nguồn, không bịa kết quả**.

---

## 1. BẢNG TỔNG HỢP CHẤM ĐIỂM 50 TIÊU CHÍ (THANG ĐIỂM 100)

| STT | Tiêu chí đánh giá (Rút gọn) | Điểm | Bằng chứng kiểm chứng (Code file:dòng / Request Runtime HTTP) | Đề xuất khắc phục / Cần sửa gì (1 dòng) |
| :---: | :--- | :---: | :--- | :--- |
| **1** | Bố cục & Quy chuẩn văn bản Word | **CHƯA CHẤM** | File nộp là mẫu gốc `Mau_Bao_Cao-...docx` (chưa điền nội dung đề tài Baby Care Store). | Điền thông tin đề tài, MSSV, mục lục, bảng biểu vào file mẫu của trường. |
| **2** | Tổng quan đề tài & Lý thuyết (Chương 1) | **CHƯA CHẤM** | Chưa có văn bản báo cáo hoàn chỉnh để chấm lý thuyết. | Hoàn thiện Chương 1 về bối cảnh Mẹ & Bé, CLR, RESTful, JWT, WinForms. |
| **3** | Phân tích yêu cầu & Thiết kế (Chương 2) | **CHƯA CHẤM** | Chưa có văn bản báo cáo chứa sơ đồ kiến trúc 3 lớp, DFD, ERD. | Vẽ sơ đồ kiến trúc 3 lớp, DFD và ERD 6 bảng kèm bảng đặc tả trường. |
| **4** | Triển khai tầng Backend Web API (Chương 3) | **CHƯA CHẤM** | Chưa có văn bản báo cáo mô tả kiến trúc Controller, DI, EF Core, ADO.NET. | Trình bày kiến trúc Web API, JWT Bearer, Transaction và Data Seeding. |
| **5** | Triển khai ứng dụng máy trạm WinForms (Chương 4) | **CHƯA CHẤM** | Chưa có văn bản báo cáo mô tả giao diện Shell, POS, bảng đặc tả API. | Thêm bảng danh mục endpoint API (URL, Method, JSON mẫu) vào Chương 4. |
| **6** | Kịch bản & Kết quả kiểm thử (Chương 5) | **CHƯA CHẤM** | Chưa có văn bản báo cáo ghi lại 54 ca kiểm thử tự động. | Chụp ảnh và đưa bảng 54 test cases kiểm thử API/WinForms vào Chương 5. |
| **7** | Đánh giá & Hướng phát triển (Chương 6) | **CHƯA CHẤM** | Chưa có văn bản kết luận và hướng mở rộng (thanh toán online, mobile). | Viết đánh giá ưu nhược điểm và hướng phát triển cổng VNPay/MoMo. |
| **8** | Phụ lục kỹ thuật & Mã nguồn | **CHƯA CHẤM** | Chưa có văn bản phụ lục liệt kê danh mục mã vạch và script SQL. | Đính kèm script tạo bảng, Stored Procedure và link GitHub vào phụ lục. |
| **9** | Lịch sử Git tối thiểu 5 commit theo buổi | **2** | `git log --all --oneline`: 24+ commit trên các nhánh `buoi2`, `buoi3`, `buoi4`, `buoi5`, `buoi6`, `master`. | Đã đáp ứng xuất sắc quy chuẩn phân chia lịch sử Git theo từng buổi. |
| **10** | README.md hướng dẫn & .gitignore loại bỏ rác | **1** | README.md đầy đủ (4.025 ký tự), nhưng `git ls-files` vẫn còn 128 file rác trong `bin/` và `obj/` bị lưu trong Git. | Chạy `git rm -r --cached **/bin **/obj` rồi commit để dọn sạch cache Git. |
| **11** | Cấu trúc Solution 2 Project & CORS Program.cs | **1** | Program.cs có DI và Swagger, nhưng **THIẾU HOÀN TOÀN CORS** (`AddCors` & `UseCors`). Solution có thêm `ThucHanhAdoNet`. | Thêm `builder.Services.AddCors()` và `app.UseCors()` trong `Program.cs`. |
| **12** | Chuỗi kết nối trong appsettings.json | **2** | appsettings.json:12 khai báo `DefaultConnection`; Program.cs:13 nạp qua DI `GetConnectionString`. | Đã lưu tập trung đúng chuẩn, không hardcode trong mã C#. |
| **13** | Controller chuyên biệt & Route RESTful | **1** | File CategoryController.cs đặt tên file số ít (dù class là `CategoriesController`); còn sót `WeatherForecastController.cs`. | Đổi tên file thành `CategoriesController.cs` và xóa `WeatherForecastController`. |
| **14** | Sử dụng đúng phương thức & mã HTTP | **2** | GET trả 200, POST tạo mới trả 201 Created kèm Header Location, POST rỗng trả 400 Bad Request, GET id sai trả 404, DELETE trả 204. | Đã tuân thủ chuẩn mã trạng thái HTTP RESTful. |
| **15** | Sử dụng DTO truyền nhận dữ liệu | **1** | Có nhiều DTO chuẩn, nhưng CategoryController.cs:24 trả nguyên `List<Category>`, và một số hàm POST nhận trực tiếp Entity `Customer`, `Product`. | Tạo thêm `CategoryDto`, `CreateProductDto` để tránh lộ trực tiếp Model Entity. |
| **16** | Lập trình bất đồng bộ (async / await) | **1** | Hầu hết là async/await, nhưng AdoProductController.cs:47 (4 actions) và CheckoutController.cs:41 còn chạy đồng bộ `public IActionResult`. | Chuyển sang `async Task<IActionResult>` và dùng `ExecuteReaderAsync()`. |
| **17** | Kiểm thử trực tiếp trên Swagger UI | **2** | `GET /swagger/index.html` trả về HTTP 200; `GET /swagger/v1/swagger.json` trả về HTTP 200, thử nghiệm các API thông suốt. | Swagger hoạt động mượt mà, tài liệu hóa đầy đủ các schema. |
| **18** | Đủ 6 Model Entity theo nghiệp vụ | **2** | Đủ 6 class Entity chuẩn trong `Models/`: AppUser.cs, Category.cs, Product.cs, Customer.cs, Order.cs, OrderDetail.cs. | Đầy đủ 100% các thực thể theo mô hình cơ sở dữ liệu. |
| **19** | Cấu hình DbContext & Khóa ngoại | **2** | SupermarketDbContext.cs:15-51 có 6 DbSet; Fluent API cấu hình Cascade cho OrderDetail, SetNull cho Customer, Restrict cho Product. | Đã cấu hình quan hệ 1-N và khóa ngoại chặt chẽ. |
| **20** | Ràng buộc kiểm tra dữ liệu Entity | **1** | Có `[Required]`, `[StringLength]` nhưng Product.cs:22 **THIẾU `[Range]`** để kiểm tra giá bán `Price > 0`. | Bổ sung `[Range(100, 100000000)]` trên thuộc tính `Price` của `Product.cs`. |
| **21** | Tạo CSDL bằng EF Core Migrations | **2** | Thư mục `Migrations/` có 8 migration; bảng `__EFMigrationsHistory` trong CSDL có 8 bản ghi tương ứng từ khởi tạo đến cập nhật. | Đã dùng chuẩn Add-Migration và Update-Database. |
| **22** | Nạp sẵn dữ liệu mẫu (Data Seeding) | **2** | SQL Server thật đếm được: 50 Products (yêu cầu >= 15), 15 Categories (yêu cầu >= 5), 15 Users, 15 Customers, 15 Orders. | Vượt chỉ tiêu dữ liệu mồi với chủ đề Siêu thị Mẹ & Bé Baby Care. |
| **23** | Thao tác Thêm, Sửa, Xóa (CRUD) qua EF Core | **2** | Đã test thực tế: POST tạo danh mục mới trả HTTP 201 ID 3023, PUT sửa tên trả 204, DELETE xóa trả 204 thành công qua EF Core. | CRUD qua EF Core chạy ổn định, đồng bộ dữ liệu tức thì. |
| **24** | Lọc và Tìm kiếm bằng LINQ | **2** | ProductsController.cs:61-70 dùng `ProductName.ToLower().Contains()` và lọc `CategoryId`; test `keyword=Bình&categoryId=5` trả về đúng 3 sản phẩm. | Tìm kiếm gần đúng và lọc danh mục hoạt động chính xác. |
| **25** | Phân trang dữ liệu máy chủ (Server-side) | **2** | ProductsController.cs:81-82 áp dụng `.Skip().Take()`; trả về `TotalItems`, `TotalPages`; gọi trang vượt ngưỡng `pageNumber=9999` trả mảng rỗng không lỗi. | Phân trang trực tiếp dưới SQL Server tối ưu hiệu năng. |
| **26** | Nạp bảng liên quan Eager Loading (.Include) | **2** | ProductsController.cs:54 dùng `.Include(p => p.Category)`; trường `CategoryName` trong DTO luôn có dữ liệu hợp lệ (không null). | Đã giải quyết triệt để vấn đề N+1 Query. |
| **27** | Tối ưu truy vấn với AsNoTracking() | **1** | Tất cả GET danh sách lớn (`GetPagedProducts`, `GetQuickReport`, `GetAll`) đều có `.AsNoTracking()`. Tuy nhiên, `CategoryController.GetById` và `ProductsController.GetById` còn thiếu. | Thêm `.AsNoTracking()` vào các câu truy vấn GET chi tiết đơn lẻ. |
| **28** | Đăng nhập cấp mã Token JWT | **2** | Gọi `POST /api/auth/login` với tài khoản `admin01 / 123456` trả về HTTP 200, chuỗi JWT Token hợp lệ dài 203 ký tự kèm `role: Admin`. | Cấp phát Bearer Token chuẩn thuật toán HmacSha256. |
| **29** | Chặn API khi chưa đăng nhập (401) | **2** | Gắn `[Authorize]` trên Controller; gọi `GET /api/categories` không có Token trả về đúng `HTTP 401 Unauthorized` kèm header `WWW-Authenticate: Bearer`. | Cơ chế bảo vệ Endpoint hoạt động chuẩn mực. |
| **30** | Phân quyền theo vai trò (Role-based) | **2** | `GET /api/users` gắn `[Authorize(Roles = "Admin")]`: Admin gọi trả HTTP 200; Cashier và Warehouse gọi trả đúng `HTTP 403 Forbidden`. | Phân quyền RBAC hoạt động chính xác theo 3 vai trò. |
| **31** | Gắn Token khi kiểm thử Swagger | **2** | Program.cs:22-35 cấu hình Security Definition Bearer; Swagger UI có nút ổ khóa "Authorize", dán JWT gọi thông suốt các API bảo mật. | Đã tích hợp UI bảo mật trên Swagger theo chuẩn OpenAPI. |
| **32** | Tra cứu Barcode bằng ADO.NET | **2** | `GET /api/ado-products/barcode/MB001001` qua `SqlConnection`/`SqlCommand` trả về HTTP 200 thông tin sản phẩm; barcode sai trả đúng HTTP 404. | Tra cứu mã vạch ADO.NET thuần phản hồi nhanh chóng. |
| **33** | Đọc bằng SqlDataReader & Chống SQL Injection | **2** | AdoProductController.cs:64-75 dùng `SqlDataReader` bọc trong `using`; toàn bộ tham số dùng `Parameters.AddWithValue`, không cộng chuỗi SQL. | Chống Injection tuyệt đối và giải phóng tài nguyên bằng `using`. |
| **34** | Chạy Stored Procedure báo cáo | **1** | Đã tạo thủ tục `sp_GetRevenueByCashier` trong file script `Buoi5_StoredProcedures.sql`, nhưng trong ReportsController.cs:128 lại dùng LINQ EF Core thay vì `CommandType.StoredProcedure`. | Chuyển code C# trong action `revenue-by-cashier` sang gọi SP bằng ADO.NET. |
| **35** | Thanh toán bọc trong Transaction & Rollback | **2** | CheckoutController.cs:57-220 bọc trong `SqlTransaction`; test mua vượt tồn kho (99999) lập tức Rollback và trả HTTP 400, không sinh đơn rác. | Đảm bảo tính toàn vẹn dữ liệu chuẩn ACID. |
| **36** | Giao diện Shell phân vùng cố định | **2** | FormMainShell.cs:54-172 thiết kế Sidebar trái (`DockStyle.Left`), TopHeader trên (`DockStyle.Top`), Panel nội dung chính giữa (`DockStyle.Fill`). | Giao diện công nghiệp chuẩn mực, không vỡ layout khi phóng to. |
| **37** | Đổi màn hình nạp mượt mà vào Panel | **2** | FormMainShell.cs:219-224 phương thức `OpenChildForm` gán `TopLevel = false`, `Dock = Fill`, `panelMainContent.Controls.Clear()`. | Chuyển đổi màn hình mượt mà, không giật lag, không bật cửa sổ rời. |
| **38** | Màn hình Đăng nhập lưu Token biến tĩnh | **2** | SessionManager.cs:14-18, 56-60 lưu `JwtToken`, `CurrentRole`, `CurrentUsername` vào biến `static`, tự động cấu hình Bearer Header. | Quản lý phiên làm việc tập trung trong suốt vòng đời WinForms. |
| **39** | Ẩn/Hiện nút bấm theo quyền tài khoản | **2** | FormMainShell.cs:253-276 `ApplyRolePermissions`: Cashier chỉ thấy POS & Khách hàng; Warehouse chỉ thấy Danh mục & Sản phẩm; Admin thấy toàn bộ. | Kiểm soát phân quyền 2 lớp (ẩn menu và chặn ở sự kiện Click). |
| **40** | Gọi API tập trung qua HttpClient | **2** | SessionManager.cs:24-40 dùng chung `static readonly HttpClient Client`, hàm `AttachToken()` tự động gắn `Authorization: Bearer <Token>`. | Tối ưu Connection Pool, tránh lỗi cạn kiệt Socket (Socket Exhaustion). |
| **41** | Bảng danh mục DataGridView tải từ API | **2** | FormProductManagement.cs:557-626 `dgvProducts.DataSource = result.Items`; nạp phân trang từ API, các nút Thêm/Sửa/Xóa gọi trực tiếp Web API. | Hiển thị bảng dữ liệu chuyên nghiệp, căn lề số lượng/giá bán chuẩn. |
| **42** | Quầy POS tính tiền & Bắt lỗi thân thiện | **2** | FormPOS.cs:741-1150 tự cộng dồn số lượng, tính tổng tiền, tính tiền thối lại; bọc try/catch hiển thị MessageBox tiếng Việt rõ ràng, không crash. | Nghiệp vụ bán hàng hoàn chỉnh, trải nghiệm thu ngân tiện dụng. |
| **43** | Cảnh báo Hàng sắp hết kho (CellFormatting) | **0** | FormProductManagement.cs:667 ẩn cột `IsLowStock`; **chưa bắt sự kiện `CellFormatting`** để tô màu nền đỏ/cam khi `StockQuantity <= 10`. | Thêm sự kiện `dgvProducts.CellFormatting` đổi màu nền đỏ khi tồn kho <= 10. |
| **44** | Dashboard Thẻ chỉ số tổng quan (4 KPI) | **1** | FormQuickReport.cs có 4 thẻ KPI nhưng 2 chỉ số khác tiêu chí (Hóa đơn TB, Điểm tích lũy thay vì Khách mới, Hàng sắp hết); API đặt tên `/api/reports/quick`. | Đổi tên route API thành `/api/reports/dashboard-summary` và chuẩn hóa 4 chỉ số. |
| **45** | Biểu đồ Cột Doanh thu (Bar Chart) | **0** | API chưa viết SP gom nhóm theo `CAST(OrderDate AS DATE)`; WinForms **chưa có điều khiển `Chart`** (`DataVisualization.Charting.Chart`) vẽ cột tự động. | Kéo điều khiển Chart vào `FormQuickReport` và viết SP GROUP BY ngày. |
| **46** | Thống kê Top 5 Sản phẩm Bán chạy | **2** | ReportsController.cs:259-269 dùng LINQ `GroupBy`, `OrderByDescending(Sum Quantity)`, `Take(limit)`; WinForms hiển thị bảng Top sản phẩm. | Đã hiện thực trọn vẹn cả tầng Backend lẫn Frontend. |
| **47** | Đổi Điểm Thưởng Trừ Tiền Hóa Đơn | **0** | CheckoutController.cs:115 chỉ mới có nghiệp vụ **tích lũy điểm** (10.000đ = 1 điểm), **chưa có nghiệp vụ đổi điểm trừ tiền** vào `FinalAmount`. | Bổ sung `PointsToRedeem` vào request và trừ tiền trong SqlTransaction. |
| **48** | Xuất Báo cáo Danh mục & Hóa đơn Excel/CSV | **0** | Chưa cài đặt thư viện ClosedXML/EPPlus ở API và chưa có chức năng xuất dữ liệu ra file `.csv` ở các màn hình WinForms. | Thêm nút "Xuất CSV" trong `FormProductManagement` dùng `StreamWriter`. |
| **49** | Kịch bản Demo trơn tru lúc báo cáo | **CHƯA CHẤM** | Cần sinh viên thực hiện trực tiếp trong buổi bảo vệ trước Hội đồng. | Thực hiện theo kịch bản mẫu 7-10 phút được hướng dẫn ở Mục 4. |
| **50** | Trả lời câu hỏi vấn đáp bảo vệ | **CHƯA CHẤM** | Cần sinh viên trả lời trực tiếp các câu hỏi của Giảng viên chấm thi. | Ôn luyện kỹ 15 câu hỏi trọng tâm kèm vị trí code ở Mục 4. |

---

## 2. TỔNG KẾT ĐIỂM SỐ & DỰ BÁO XẾP LOẠI

```
┌────────────────────────────────────────────────────────────────────────┐
│                        KẾT QUẢ ĐÁNH GIÁ ĐỒ ÁN                          │
├────────────────────────────────────────────────────────────────────────┤
│  • Tổng số tiêu chí đánh giá:               50 tiêu chí                │
│  • Số tiêu chí đã kiểm chứng kỹ thuật:       40 tiêu chí (80 điểm)      │
│  • Số tiêu chí CHƯA CHẤM (Báo cáo & Demo):   10 tiêu chí (20 điểm)      │
├────────────────────────────────────────────────────────────────────────┤
│  ► ĐIỂM ĐÃ ĐẠT ĐƯỢC (trên phần đã chấm):     63 / 80 điểm (78.75%)     │
│  ► ĐIỂM DỰ KIẾN TỐI ĐA (khi hoàn tất Word):   83 / 100 điểm (Loại Giỏi) │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 3. DANH SÁCH CẦN SỬA ĐỂ TĂNG ĐIỂM (XẾP THEO "LỢI NHIỀU ĐIỂM - ÍT CÔNG SỨC")

1. **Tiêu chí 11 (+1 điểm) — Bật CORS trong `Program.cs`**:
   - Thêm `builder.Services.AddCors(...)` và `app.UseCors()` vào `Program.cs`. (Mất 2 phút).
2. **Tiêu chí 10 (+1 điểm) — Xóa 128 file rác `bin/` và `obj/` khỏi Git**:
   - Chạy `git rm -r --cached **/bin **/obj` rồi commit. (Mất 2 phút).
3. **Tiêu chí 43 (+2 điểm) — Tô màu cảnh báo tồn kho <= 10 bằng `CellFormatting`**:
   - Thêm sự kiện `dgvProducts.CellFormatting` trong `FormProductManagement.cs` đổi màu nền đỏ khi tồn kho <= 10. (Mất 5 phút).
4. **Tiêu chí 20 (+1 điểm) — Thêm ràng buộc giá bán > 0 vào Entity**:
   - Đặt `[Range(100, 100000000)]` trên thuộc tính `Price` của `Product.cs`. (Mất 2 phút).
5. **Tiêu chí 48 (+2 điểm) — Thêm nút "Xuất CSV" trong Quản lý sản phẩm**:
   - Thêm nút `btnExportCsv` trong `FormProductManagement.cs` ghi các dòng DataGridView ra file `.csv`. (Mất 10 phút).
6. **Tiêu chí 13 (+1 điểm) — Chuẩn hóa tên file Controller**:
   - Đổi tên `CategoryController.cs` thành `CategoriesController.cs` và xóa `WeatherForecastController.cs`. (Mất 2 phút).
7. **Tiêu chí 34 (+1 điểm) — Gọi Stored Procedure bằng ADO.NET thuần**:
   - Sửa action `revenue-by-cashier` dùng `CommandType.StoredProcedure` gọi `sp_GetRevenueByCashier`. (Mất 10 phút).

---

## 4. CHECKLIST BẤM TAY WINFORMS, KỊCH BẢN DEMO & 15 CÂU HỎI VẤN ĐÁP

### A. Checklist Kiểm thử Thủ công Màn hình WinForms
- Bước 1: Mở app, đăng nhập `cashier01` / `123456` -> Menu chỉ có POS & Khách hàng.
- Bước 2: Đăng xuất, đăng nhập `ware01` / `123456` -> Menu chỉ có Danh mục & Sản phẩm.
- Bước 3: Đăng xuất, đăng nhập `admin01` / `123456` -> Menu đầy đủ 6 nút.
- Bước 4: Bấm chuyển các form con -> Nạp mượt mà trong Panel giữa.
- Bước 5: Mở Quản lý Sản phẩm -> Tìm kiếm `bột`, phân trang trang 1, trang 2.
- Bước 6: Mở POS -> Quét `MB001001` (Sữa NAN) 2 lần -> Giỏ hàng cộng dồn, nhập tiền khách đưa -> Ra tiền thối.
- Bước 7: Bấm Thanh toán -> Thông báo thành công tiếng Việt, giỏ hàng làm mới.

---

### B. Kịch bản Demo Báo cáo Đồ án (7 - 10 Phút)
- Phút 0 - 1: Giới thiệu đề tài Baby Care Store, chỉ rõ 2 project trong Solution, mở Swagger UI.
- Phút 1 - 3: Chứng minh JWT & RBAC (Swagger 401 khi không token, đăng nhập WinForms tài khoản Cashier và Admin).
- Phút 3 - 6: Bán hàng tại POS (quét `MB001001`, chọn khách hàng tích điểm, nhập tiền khách, bấm Thanh toán).
- Phút 6 - 8: Đối chiếu SQL Server thật (Order tạo mới, trừ tồn kho từ 45 xuống 43, cộng điểm tích lũy trong cùng 1 SqlTransaction).
- Phút 8 - 10: Xem Báo cáo Top 5 sản phẩm bán chạy và Doanh thu ca.

---

### C. 15 Câu hỏi Vấn đáp Trọng tâm
1. Tại sao tách riêng Web API và WinForms? -> Bảo mật CSDL sau tường lửa, cấp token JWT, tái sử dụng API cho Mobile/Web.
2. Token JWT hoạt động như thế nào? -> Sinh token chứa Claim, ký HmacSha256, WinForms lưu biến tĩnh và gắn Header Bearer.
3. Chặn quyền Thu ngân vào xem Báo cáo ra sao? -> Gắn `[Authorize(Roles = "Admin")]` trên ReportsController, trả 403 Forbidden.
4. Tại sao tra cứu Barcode lại dùng ADO.NET? -> SqlDataReader đọc nhanh dưới 50ms, không tốn overhead ánh xạ đối tượng của EF Core.
5. Chống SQL Injection thế nào? -> Dùng `cmd.Parameters.AddWithValue`, tuyệt đối không cộng chuỗi SQL.
6. Thanh toán Transaction giải quyết bài toán gì? -> Bảo vệ tính nguyên tử ACID: lưu đơn, trừ kho, tích điểm hoặc Rollback toàn bộ khi thiếu hàng.
7. Lỗi N+1 Query và cách khắc phục? -> Dùng `.Include(p => p.Category)` để Eager Loading JOIN 2 bảng trong 1 truy vấn.
8. Phân trang Server-side hoạt động ra sao? -> Dùng LINQ `.Skip().Take()`, dịch sang SQL Server `OFFSET ... FETCH NEXT`.
9. `.AsNoTracking()` có tác dụng gì? -> Tắt Change Tracker, tiết kiệm RAM và tăng tốc 30% cho truy vấn chỉ đọc.
10. Tại sao bọc SqlDataReader trong using? -> Đảm bảo gọi Dispose() đóng kết nối ngay lập tức, chống rò rỉ Connection Pool.
11. WinForms nạp form con vào Panel thế nào không lag? -> `childForm.TopLevel = false`, `Dock = Fill`, `panel.Controls.Add(childForm)`.
12. Mật khẩu lưu dạng nào trong CSDL? -> Băm bảo mật PBKDF2 kèm muối Salt ngẫu nhiên, không lưu plain text.
13. Tại sao dùng 1 HttpClient static duy nhất? -> Tránh cạn kiệt cổng mạng (Socket Exhaustion) do TIME_WAIT.
14. Xóa khách hàng thì hóa đơn cũ có mất không? -> Không mất, khóa ngoại cấu hình `OnDelete(DeleteBehavior.SetNull)` biến thành khách vãng lai.
15. Hướng phát triển tiếp theo? -> Tích hợp cổng VietQR/VNPay tại quầy POS và xây dựng App tra cứu điểm thưởng cho khách.
