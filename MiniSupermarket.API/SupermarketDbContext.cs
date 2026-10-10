using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Data
{
    // DbContext đại diện cho phiên làm việc với cơ sở dữ liệu SQL Server
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(DbContextOptions<SupermarketDbContext> options) : base(options) { }

        // Khai báo các bảng dữ liệu ánh xạ từ Model
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        // Cấu hình dữ liệu mồi ban đầu (Data Seeding)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Tên đăng nhập không được trùng nhau
            modelBuilder.Entity<AppUser>().HasIndex(u => u.Username).IsUnique();
            // Xóa khách hàng thì hóa đơn cũ vẫn giữ lại (chuyển thành khách lẻ)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany()
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            // Xóa hóa đơn thì xóa luôn các dòng hàng của nó
            modelBuilder.Entity<OrderDetail>()
                .HasOne(d => d.Order)
                .WithMany(o => o.Details)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Sản phẩm đã từng bán thì không được xóa
            modelBuilder.Entity<OrderDetail>()
                .HasOne(d => d.Product)
                .WithMany()
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Nạp sẵn 15 danh mục của Siêu thị Mini Mẹ & Bé Baby Care Store
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId = 1, CategoryName = "Sữa bột & Sữa công thức", Description = "Sữa bột cho bé theo độ tuổi, sữa công thức đặc biệt" },
                new Category { CategoryId = 2, CategoryName = "Sữa tươi & Sữa dinh dưỡng cho bé", Description = "Sữa tươi, sữa chua uống, sữa hạt cho trẻ nhỏ" },
                new Category { CategoryId = 3, CategoryName = "Bỉm, Tã & Khăn ướt", Description = "Bỉm dán, tã quần, khăn ướt, giấy ướt cho bé" },
                new Category { CategoryId = 4, CategoryName = "Đồ ăn dặm & Bột ăn dặm", Description = "Bột ăn dặm, cháo tươi, bánh ăn dặm, váng sữa" },
                new Category { CategoryId = 5, CategoryName = "Bình sữa & Núm ty", Description = "Bình sữa, núm ty, ti giả, dụng cụ vệ sinh bình sữa" },
                new Category { CategoryId = 6, CategoryName = "Đồ dùng ăn uống cho bé", Description = "Chén, muỗng, ly tập uống, yếm ăn, ghế ăn dặm" },
                new Category { CategoryId = 7, CategoryName = "Tắm gội & Chăm sóc da bé", Description = "Sữa tắm, dầu gội, kem chống hăm, phấn rôm, dầu massage" },
                new Category { CategoryId = 8, CategoryName = "Vệ sinh & Chăm sóc răng miệng bé", Description = "Bàn chải, kem đánh răng, tưa lưỡi, nhiệt kế, bấm móng tay" },
                new Category { CategoryId = 9, CategoryName = "Quần áo & Phụ kiện sơ sinh", Description = "Bodysuit, đồ sơ sinh, tất, mũ, bao tay, bao chân" },
                new Category { CategoryId = 10, CategoryName = "Chăn, Gối & Đồ ngủ", Description = "Chăn, gối chặn, nệm, màn, khăn quấn ủ cho bé" },
                new Category { CategoryId = 11, CategoryName = "Đồ chơi & Học liệu", Description = "Đồ chơi gặm nướu, xúc xắc, sách vải, đồ chơi phát triển trí tuệ" },
                new Category { CategoryId = 12, CategoryName = "Xe đẩy, Địu & Ghế ngồi ô tô", Description = "Xe đẩy, địu em bé, nôi, ghế ngồi ô tô an toàn" },
                new Category { CategoryId = 13, CategoryName = "Dinh dưỡng & Vitamin cho mẹ bầu", Description = "Sữa bầu, vitamin tổng hợp, canxi, sắt, DHA cho mẹ" },
                new Category { CategoryId = 14, CategoryName = "Chăm sóc mẹ sau sinh", Description = "Đai bụng, kem rạn da, miếng lót sau sinh, đồ dùng cho mẹ" },
                new Category { CategoryId = 15, CategoryName = "Máy hút sữa & Trữ sữa", Description = "Máy hút sữa, túi trữ sữa, miếng lót thấm sữa, phễu hút" }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId = 1, CustomerName = "Nguyễn Văn A", PhoneNumber = "0901122334", MembershipRank = "Vàng", RewardPoints = 150 },
                new Customer { CustomerId = 2, CustomerName = "Trần Thị B", PhoneNumber = "0918877665", MembershipRank = "Bạc", RewardPoints = 50 },
                new Customer { CustomerId = 3, CustomerName = "Lê Văn C", PhoneNumber = "0983344556", MembershipRank = "Chuẩn", RewardPoints = 10 }
            );
        }
    }
}