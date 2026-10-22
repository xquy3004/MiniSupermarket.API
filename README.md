# Hệ Thống Quản Lý Siêu Thị Mini (MiniSupermarket)
> **Đồ án môn học**: Lập trình Ứng dụng .NET Core  
> **Chủ đề**: Cửa hàng Mẹ và Bé (Baby Care Store)  
> **Sinh viên thực hiện**: Nguyễn Xuân Quý  
> **MSSV**: 2124110252 | **Lớp**: CCQ2411C  
> **Nhánh**: `buoi3` (Bài thực hành Buổi 3)

---

## 🗄 BÁO CÁO THỰC HÀNH BUỔI 3: ENTITY FRAMEWORK CORE, SQL SERVER & QUẢN LÝ KHÁCH HÀNG

### 1. Mục tiêu buổi học
- Chuyển đổi dữ liệu từ bộ nhớ RAM (In-Memory) sang lưu trữ vĩnh viễn trên **SQL Server** sử dụng **Entity Framework Core (Code-First)**.
- Xây dựng `SupermarketDbContext`, quản lý Migration và cấu hình chuỗi kết nối an toàn.
- Quản lý thực thể Khách hàng thân thiết (`Customer`): Thêm, Sửa, Xóa, Tìm kiếm và tích điểm thành viên.
- Cấu hình toàn vẹn dữ liệu quan hệ: Thiết lập cơ chế `OnDelete(DeleteBehavior.SetNull)` khi xóa khách hàng.

---

### 2. Mô hình thực thể & Cơ sở dữ liệu

#### 2.1. Thực thể `Customer.cs`
```csharp
[Table("Customers")]
public class Customer
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CustomerId { get; set; }

    [Required(ErrorMessage = "Tên khách hàng không được để trống!")]
    [StringLength(100)]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại không được để trống!")]
    [StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [StringLength(200)]
    public string? Address { get; set; }

    public int RewardPoints { get; set; } = 0;

    [StringLength(50)]
    public string MembershipRank { get; set; } = "Chuẩn";
}
```

#### 2.2. Ràng buộc quan hệ trong `SupermarketDbContext.cs`
- Khi khách hàng bị xóa khỏi hệ thống, các hóa đơn cũ đã từng mua hàng của khách đó **vẫn phải được lưu giữ** để bảo đảm tính chính xác của sổ sách doanh thu, trường `CustomerId` trong bảng `Orders` sẽ tự động chuyển thành `NULL` (Khách lẻ):
  ```csharp
  modelBuilder.Entity<Order>()
      .HasOne(o => o.Customer)
      .WithMany()
      .HasForeignKey(o => o.CustomerId)
      .OnDelete(DeleteBehavior.SetNull);
  ```

---

### 3. Backend API (`CustomersController.cs`)
| Phương thức | Đường dẫn API | Chức năng | Phân quyền |
| :---: | :--- | :--- | :---: |
| **GET** | `/api/customers` | Lấy danh sách khách hàng | `[Authorize]` |
| **GET** | `/api/customers/{id}` | Lấy chi tiết khách hàng theo ID | `[Authorize]` |
| **GET** | `/api/customers/search?keyword=...` | Tìm kiếm theo Tên hoặc Số điện thoại | `[Authorize]` |
| **POST** | `/api/customers` | Thêm khách hàng mới (Kiểm tra validate) | `[Authorize]` |
| **PUT** | `/api/customers/{id}` | Cập nhật thông tin khách hàng | `[Authorize]` |
| **DELETE** | `/api/customers/{id}` | Xóa khách hàng khỏi hệ thống | `[Authorize(Roles = "Admin")]` |

---

### 4. Giao diện WinForms (`FormCustomerManagement.cs`)
- Hiển thị danh sách khách hàng lên DataGridView gồm: Tên khách hàng, Số điện thoại, Địa chỉ, Điểm tích lũy, Hạng thành viên.
- Chức năng tìm kiếm tức thời theo Họ tên hoặc Số điện thoại.
- Nút Thêm, Cập nhật thông tin, Nâng hạng thành viên và Xóa khách hàng (Admin).
