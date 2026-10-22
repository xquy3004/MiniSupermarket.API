# Hệ Thống Quản Lý Siêu Thị Mini (MiniSupermarket)
> **Đồ án môn học**: Lập trình Ứng dụng .NET Core  
> **Chủ đề**: Cửa hàng Mẹ và Bé (Baby Care Store)  
> **Sinh viên thực hiện**: Nguyễn Xuân Quý  
> **MSSV**: 2124110252 | **Lớp**: CCQ2411C  
> **Nhánh**: `buoi5` (Bài thực hành Buổi 5)

---

## ⚡ BÁO CÁO THỰC HÀNH BUỔI 5: ADO.NET, STORED PROCEDURES & GIAO DỊCH POS (ACID TRANSACTION)

### 1. Mục tiêu buổi học
- Kết hợp công nghệ truy vấn dữ liệu hiệu năng cao **ADO.NET thuần** song song với Entity Framework Core.
- Áp dụng cả 2 chế độ: **Connected Mode** (`SqlDataReader`) và **Disconnected Mode** (`SqlDataAdapter`, `DataTable`).
- Viết và tích hợp **3 Stored Procedures** trên SQL Server phục vụ phân tích báo cáo doanh thu.
- Lập trình tính năng Thanh toán quầy POS bọc trong **`SqlTransaction` chuẩn ACID**: đảm bảo nguyên tử, Rollback an toàn tuyệt đối khi mua vượt tồn kho và tính năng Hủy hóa đơn hoàn kho.

---

### 2. Các phân hệ kỹ thuật cốt lõi

#### 2.1. ADO.NET Controller (`AdoProductController.cs`)
- **Connected Mode (SqlDataReader)**:
  - `GET /api/ado-products/barcode/{barcode}`: Quét tra cứu nhanh sản phẩm tại quầy thu ngân.
  - `GET /api/ado-products/filter-stock?minStock=...`: Lọc nhanh danh sách sản phẩm chạm ngưỡng sắp hết hàng.
- **Disconnected Mode (SqlDataAdapter & DataTable đệm RAM)**:
  - `GET /api/ado-products/disconnected-cache`: Tải toàn bộ danh mục sản phẩm vào DataTable trên RAM, ngắt kết nối vật lý ngay lập tức để tiết kiệm tài nguyên kết nối Database.
  - `GET /api/ado-products/category-count`: Thống kê tổng số lượng mặt hàng theo từng nhóm danh mục (`GROUP BY`).

#### 2.2. Báo cáo Stored Procedure (`ReportsController.cs`)
Đã triển khai 3 Stored Procedure trong file `MiniSupermarket.API/Scripts/Buoi5_StoredProcedures.sql`:
1. `sp_GetRevenueByCashier`: Báo cáo doanh thu, số đơn hàng và tổng điểm thưởng theo từng nhân viên thu ngân.
2. `sp_GetRevenueByDateRange`: Báo cáo doanh thu theo khoảng thời gian (`@FromDate` đến `@ToDate`).
3. `sp_GetTop5SellingProducts`: Báo cáo 5 mặt hàng bán chạy nhất (tính theo tổng số lượng bán và tổng tiền).

#### 2.3. Thanh toán POS bọc trong `SqlTransaction` (`CheckoutController.cs`)
- **Endpoint**: `POST /api/checkout`
- **Quy trình giao dịch khép kín (ACID)**:
  1. Mở `connection.BeginTransaction()`.
  2. Khóa và kiểm tra tồn kho từng sản phẩm (`SELECT StockQuantity`). Nếu yêu cầu mua $>$ tồn kho $ightarrow$ Kích hoạt `Rollback()` và trả về lỗi.
  3. Thêm bản ghi Hóa đơn tổng thể vào bảng `Orders` (lấy `OrderId` sinh tự động qua `OUTPUT INSERTED.OrderId`).
  4. Duyệt từng món hàng: Thêm vào bảng `OrderDetails` và trừ tồn kho trong bảng `Products`.
  5. Cộng điểm thưởng tích lũy cho khách hàng thành viên (`UPDATE Customers`).
  6. Gọi `transaction.Commit()` xác nhận lưu vĩnh viễn.
- **Tính năng Hủy đơn hàng**: `DELETE /api/checkout/cancel/{orderId}`:
  - Đọc thông tin đơn hàng, hoàn trả tồn kho, khấu trừ lại điểm thưởng của khách và xóa sạch bản ghi `OrderDetails`, `Orders`. Nếu đơn hàng không tồn tại $ightarrow$ Trả về `404 Not Found`.
