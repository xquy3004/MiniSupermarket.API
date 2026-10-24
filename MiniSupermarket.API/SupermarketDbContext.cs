using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Data
{
    /// <summary>
    /// Ngữ cảnh cơ sở dữ liệu đại diện cho phiên làm việc với SQL Server
    /// Quản lý 6 thực thể và cấu hình Data Seeding đầy đủ cho hệ thống Cửa hàng Mẹ và Bé
    /// </summary>
    public class SupermarketDbContext : DbContext
    {
        public SupermarketDbContext(DbContextOptions<SupermarketDbContext> options) : base(options) { }

        // Khai báo 6 bảng dữ liệu của hệ thống
        public DbSet<AppUser>      Users        { get; set; }
        public DbSet<Category>     Categories   { get; set; }
        public DbSet<Product>      Products     { get; set; }
        public DbSet<Customer>     Customers    { get; set; }
        public DbSet<Order>        Orders       { get; set; }
        public DbSet<OrderDetail>  OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================================================================
            // A. CẤU HÌNH RÀNG BUỘC CHỈ MỤC DUY NHẤT (UNIQUE INDEX)
            // =========================================================================
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

            // =========================================================================
            // B. DATA SEEDING CHO 6 BẢNG (MỖI BẢNG TỐI THIỂU 15 DÒNG)
            // Chủ đề: Cửa hàng Mẹ và Bé — Baby Care Store
            // =========================================================================

            // ------------------------------------------------------------------
            // 1. DATA SEEDING: BẢNG USERS (15 nhân viên – 3 vai trò khớp đề Buổi 4)
            // Mật khẩu mặc định: 123456 (được băm PBKDF2 cố định cho HasData)
            // ------------------------------------------------------------------
            const string defaultPasswordHash = "100000.JeUlHG5fcACNOYexkxBP6g==.C/s1aX+xcAsQWA+xErOZldBd5iwYYlzn9/WrcyzGVjA=";

            modelBuilder.Entity<AppUser>().HasData(
                new AppUser { Id =  1, Username = "admin01",      PasswordHash = defaultPasswordHash, FullName = "Nguyễn Quản Trị",    Role = "Admin",     IsActive = true },
                new AppUser { Id =  2, Username = "admin02",      PasswordHash = defaultPasswordHash, FullName = "Trần Giám Đốc",     Role = "Admin",     IsActive = true },
                new AppUser { Id =  3, Username = "cashier01",    PasswordHash = defaultPasswordHash, FullName = "Lê Thu Ngân",       Role = "Cashier",   IsActive = true },
                new AppUser { Id =  4, Username = "cashier02",    PasswordHash = defaultPasswordHash, FullName = "Phạm Bán Hàng",     Role = "Cashier",   IsActive = true },
                new AppUser { Id =  5, Username = "cashier03",    PasswordHash = defaultPasswordHash, FullName = "Hoàng Thu Ngân",    Role = "Cashier",   IsActive = true },
                new AppUser { Id =  6, Username = "cashier04",    PasswordHash = defaultPasswordHash, FullName = "Vũ Thị Quầy",       Role = "Cashier",   IsActive = true },
                new AppUser { Id =  7, Username = "cashier05",    PasswordHash = defaultPasswordHash, FullName = "Đỗ Bán Lẻ",         Role = "Cashier",   IsActive = true },
                new AppUser { Id =  8, Username = "ware01",       PasswordHash = defaultPasswordHash, FullName = "Ngô Quản Kho",      Role = "Warehouse", IsActive = true },
                new AppUser { Id =  9, Username = "ware02",       PasswordHash = defaultPasswordHash, FullName = "Bùi Kiểm Kê",      Role = "Warehouse", IsActive = true },
                new AppUser { Id = 10, Username = "ware03",       PasswordHash = defaultPasswordHash, FullName = "Dương Thủ Kho",     Role = "Warehouse", IsActive = true },
                new AppUser { Id = 11, Username = "ware04",       PasswordHash = defaultPasswordHash, FullName = "Lý Nhập Hàng",      Role = "Warehouse", IsActive = true },
                new AppUser { Id = 12, Username = "admin_backup", PasswordHash = defaultPasswordHash, FullName = "Đặng Hỗ Trợ",       Role = "Admin",     IsActive = true },
                new AppUser { Id = 13, Username = "cashier06",    PasswordHash = defaultPasswordHash, FullName = "Hồ Ca Chiều",       Role = "Cashier",   IsActive = true },
                new AppUser { Id = 14, Username = "ware05",       PasswordHash = defaultPasswordHash, FullName = "Trương Vận Chuyển", Role = "Warehouse", IsActive = true },
                new AppUser { Id = 15, Username = "supervisor",   PasswordHash = defaultPasswordHash, FullName = "Mai Giám Sát",      Role = "Admin",     IsActive = true }
            );

            // ------------------------------------------------------------------
            // 2. DATA SEEDING: BẢNG CATEGORIES (15 nhóm hàng Mẹ & Bé)
            // ------------------------------------------------------------------
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryId =  1, CategoryName = "Sữa bột & Sữa công thức",        Description = "Sữa bột cho bé theo độ tuổi, sữa công thức đặc biệt" },
                new Category { CategoryId =  2, CategoryName = "Sữa tươi & Sữa dinh dưỡng bé",  Description = "Sữa tươi, sữa chua uống, sữa hạt cho trẻ nhỏ" },
                new Category { CategoryId =  3, CategoryName = "Bỉm, Tã & Khăn ướt",            Description = "Bỉm dán, tã quần, khăn ướt, giấy ướt cho bé" },
                new Category { CategoryId =  4, CategoryName = "Đồ ăn dặm & Bột ăn dặm",        Description = "Bột ăn dặm, cháo tươi, bánh ăn dặm, váng sữa" },
                new Category { CategoryId =  5, CategoryName = "Bình sữa & Núm ty",              Description = "Bình sữa, núm ty, ti giả, dụng cụ vệ sinh bình sữa" },
                new Category { CategoryId =  6, CategoryName = "Đồ dùng ăn uống cho bé",        Description = "Chén, muỗng, ly tập uống, yếm ăn, ghế ăn dặm" },
                new Category { CategoryId =  7, CategoryName = "Tắm gội & Chăm sóc da bé",      Description = "Sữa tắm, dầu gội, kem chống hăm, phấn rôm, dầu massage" },
                new Category { CategoryId =  8, CategoryName = "Vệ sinh & Chăm sóc răng miệng", Description = "Bàn chải, kem đánh răng, tưa lưỡi, nhiệt kế, bấm móng tay" },
                new Category { CategoryId =  9, CategoryName = "Quần áo & Phụ kiện sơ sinh",    Description = "Bodysuit, đồ sơ sinh, tất, mũ, bao tay, bao chân" },
                new Category { CategoryId = 10, CategoryName = "Chăn, Gối & Đồ ngủ",            Description = "Chăn, gối chặn, nệm, màn, khăn quấn ủ cho bé" },
                new Category { CategoryId = 11, CategoryName = "Đồ chơi & Học liệu",            Description = "Đồ chơi gặm nướu, xúc xắc, sách vải, đồ chơi phát triển trí tuệ" },
                new Category { CategoryId = 12, CategoryName = "Xe đẩy, Địu & Ghế ngồi ô tô",  Description = "Xe đẩy, địu em bé, nôi, ghế ngồi ô tô an toàn" },
                new Category { CategoryId = 13, CategoryName = "Dinh dưỡng & Vitamin mẹ bầu",   Description = "Sữa bầu, vitamin tổng hợp, canxi, sắt, DHA cho mẹ" },
                new Category { CategoryId = 14, CategoryName = "Chăm sóc mẹ sau sinh",          Description = "Đai bụng, kem rạn da, miếng lót sau sinh, đồ dùng mẹ" },
                new Category { CategoryId = 15, CategoryName = "Máy hút sữa & Trữ sữa",         Description = "Máy hút sữa, túi trữ sữa, miếng lót thấm sữa, phễu hút" }
            );

            // ------------------------------------------------------------------
            // 3. DATA SEEDING: BẢNG PRODUCTS (20 sản phẩm Mẹ & Bé)
            //    Barcode: MB + 3 chữ số nhóm + 3 chữ số SP
            // ------------------------------------------------------------------
            modelBuilder.Entity<Product>().HasData(
                // --- Nhóm 3: Bỉm, Tã & Khăn ướt ---
                new Product { ProductId =  1, Barcode = "893456789001", ProductName = "Tã dán Pampers Newborn size NB (dưới 5kg) – 84 miếng", Price =  195000m, StockQuantity =  85, CategoryId =  3 },
                new Product { ProductId =  2, Barcode = "893456789002", ProductName = "Tã quần Huggies Dry size M (6-11kg) – 48 miếng", Price =  185000m, StockQuantity =  70, CategoryId =  3 },
                new Product { ProductId =  3, Barcode = "893456789003", ProductName = "Tã dán Bobby Newborn size S (3-6kg) – 56 miếng", Price =  135000m, StockQuantity = 120, CategoryId =  3 },
                new Product { ProductId =  4, Barcode = "893456789004", ProductName = "Khăn ướt Bobby không mùi hộp 100 tờ", Price =   42000m, StockQuantity = 200, CategoryId =  3 },
                // --- Nhóm 1: Sữa bột & Sữa công thức ---
                new Product { ProductId =  5, Barcode = "893456789005", ProductName = "Sữa bột NAN Optipro 1 (0-6 tháng) hộp 800g", Price =  520000m, StockQuantity =  45, CategoryId =  1 },
                new Product { ProductId =  6, Barcode = "893456789006", ProductName = "Sữa bột Enfamil A+ số 2 (6-12 tháng) hộp 900g", Price =  490000m, StockQuantity =  50, CategoryId =  1 },
                new Product { ProductId =  7, Barcode = "893456789007", ProductName = "Sữa bột Aptamil số 3 (1-3 tuổi) hộp 800g", Price =  480000m, StockQuantity =  40, CategoryId =  1 },
                new Product { ProductId =  8, Barcode = "893456789008", ProductName = "Sữa bột Dielac Alpha Step 1 (0-6 tháng) hộp 900g", Price =  320000m, StockQuantity =  60, CategoryId =  1 },
                // --- Nhóm 4: Đồ ăn dặm ---
                new Product { ProductId =  9, Barcode = "893456789009", ProductName = "Bột ăn dặm Heinz vị gạo sữa loại 200g", Price =   85000m, StockQuantity =  90, CategoryId =  4 },
                new Product { ProductId = 10, Barcode = "893456789010", ProductName = "Cháo tươi ăn dặm Nutricare gà rau củ hộp 200g", Price =   38000m, StockQuantity = 150, CategoryId =  4 },
                new Product { ProductId = 11, Barcode = "893456789011", ProductName = "Váng sữa Petit Suisse Yoplait vị dâu lốc 6 hộp", Price =   65000m, StockQuantity = 100, CategoryId =  4 },
                // --- Nhóm 5: Bình sữa & Núm ty ---
                new Product { ProductId = 12, Barcode = "893456789012", ProductName = "Bình sữa Pigeon PPSU cổ rộng 160ml", Price =  185000m, StockQuantity =  55, CategoryId =  5 },
                new Product { ProductId = 13, Barcode = "893456789013", ProductName = "Bình sữa Dr. Brown's chống sặc 250ml", Price =  245000m, StockQuantity =  35, CategoryId =  5 },
                // --- Nhóm 7: Tắm gội & Chăm sóc da bé ---
                new Product { ProductId = 14, Barcode = "893456789014", ProductName = "Sữa tắm gội Pigeon 2 trong 1 hương hoa quả 200ml", Price =   95000m, StockQuantity = 110, CategoryId =  7 },
                new Product { ProductId = 15, Barcode = "893456789015", ProductName = "Kem chống hăm Bepanthen Baby 30g", Price =  125000m, StockQuantity =  80, CategoryId =  7 },
                new Product { ProductId = 16, Barcode = "893456789016", ProductName = "Dầu massage Johnson's Baby 200ml", Price =   78000m, StockQuantity =  95, CategoryId =  7 },
                // --- Nhóm 9: Quần áo sơ sinh ---
                new Product { ProductId = 17, Barcode = "893456789017", ProductName = "Bộ bodysuit cotton sơ sinh 5 bộ size 0-3 tháng", Price =  280000m, StockQuantity =  60, CategoryId =  9 },
                // --- Nhóm 13: Dinh dưỡng mẹ bầu ---
                new Product { ProductId = 18, Barcode = "893456789018", ProductName = "Sữa bầu Enfamama A+ hộp 900g", Price =  450000m, StockQuantity =  40, CategoryId = 13 },
                new Product { ProductId = 19, Barcode = "893456789019", ProductName = "Vitamin tổng hợp Blackmores Pregnancy & Breast-feeding 60 viên", Price =  380000m, StockQuantity =  30, CategoryId = 13 },
                // --- Nhóm 11: Đồ chơi & Học liệu ---
                new Product { ProductId = 20, Barcode = "893456789020", ProductName = "Bộ đồ chơi xúc xắc gặm nướu an toàn cho bé 8 món", Price =  165000m, StockQuantity =  75, CategoryId = 11 },
                // --- 30 SẢN PHẨM MẸ & BÉ MỞ RỘNG (ProductId 21 - 50) ---
                new Product { ProductId = 21, Barcode = "893456789021", ProductName = "Sữa bột Meiji số 0 (0-1 tuổi) hộp 800g nhập khẩu Nhật", Price =  530000m, StockQuantity =  35, CategoryId =  1 },
                new Product { ProductId = 22, Barcode = "893456789022", ProductName = "Sữa bột Frisolac Gold số 2 (6-12 tháng) hộp 850g", Price =  465000m, StockQuantity =  15, CategoryId =  1 },
                new Product { ProductId = 23, Barcode = "893456789023", ProductName = "Sữa tươi TH True Milk TOP KID hương dâu 180ml lốc 4", Price =   36000m, StockQuantity = 120, CategoryId =  2 },
                new Product { ProductId = 24, Barcode = "893456789024", ProductName = "Sữa chua uống tiệt trùng Yomost vị cam lốc 4 hộp 170ml", Price =   28000m, StockQuantity =  80, CategoryId =  2 },
                new Product { ProductId = 25, Barcode = "893456789025", ProductName = "Sữa hạt ngũ cốc dinh dưỡng Pororo Hàn Quốc lốc 3 hộp 190ml", Price =   55000m, StockQuantity =  40, CategoryId =  2 },
                new Product { ProductId = 26, Barcode = "893456789026", ProductName = "Tã dán Merries nội địa Nhật size M (6-11kg) gói 64 miếng", Price =  385000m, StockQuantity =  18, CategoryId =  3 },
                new Product { ProductId = 27, Barcode = "893456789027", ProductName = "Tã quần Moony Natural hữu cơ size L (9-14kg) gói 36 miếng", Price =  410000m, StockQuantity =  25, CategoryId =  3 },
                new Product { ProductId = 28, Barcode = "893456789028", ProductName = "Khăn giấy khô đa năng siêu mềm Mama hộp 200 tờ", Price =   35000m, StockQuantity = 150, CategoryId =  3 },
                new Product { ProductId = 29, Barcode = "893456789029", ProductName = "Bánh ăn dặm Gerber Graduate vị chuối dâu hộp 42g", Price =   68000m, StockQuantity =  70, CategoryId =  4 },
                new Product { ProductId = 30, Barcode = "893456789030", ProductName = "Cháo tươi Cây Thị lươn đậu xanh dinh dưỡng gói 260g", Price =   25000m, StockQuantity =  90, CategoryId =  4 },
                new Product { ProductId = 31, Barcode = "893456789031", ProductName = "Váng sữa Monte vị vani nhập khẩu Đức lốc 4 hộp 55g", Price =   62000m, StockQuantity = 110, CategoryId =  4 },
                new Product { ProductId = 32, Barcode = "893456789032", ProductName = "Núm ty thay thế Pigeon siêu mềm cổ rộng size M vỉ 2 cái", Price =  120000m, StockQuantity =  50, CategoryId =  5 },
                new Product { ProductId = 33, Barcode = "893456789033", ProductName = "Ti giả chỉnh nha Philips Avent Ultra Air ban đêm vỉ 2 cái", Price =  175000m, StockQuantity =  10, CategoryId =  5 },
                new Product { ProductId = 34, Barcode = "893456789034", ProductName = "Nước rửa bình sữa và rau củ D-nee chai có vòi 620ml", Price =   85000m, StockQuantity =  65, CategoryId =  5 },
                new Product { ProductId = 35, Barcode = "893456789035", ProductName = "Bộ chén muỗng tập ăn dặm báo nóng Richell Nhật Bản", Price =  145000m, StockQuantity =  40, CategoryId =  6 },
                new Product { ProductId = 36, Barcode = "893456789036", ProductName = "Ghế ăn dặm gấp gọn nâng hạ độ cao Mastela cho bé", Price =  490000m, StockQuantity =   8, CategoryId =  6 },
                new Product { ProductId = 37, Barcode = "893456789037", ProductName = "Yếm ăn dặm silicon cao cấp chống thấm có máng hứng", Price =   55000m, StockQuantity =  85, CategoryId =  6 },
                new Product { ProductId = 38, Barcode = "893456789038", ProductName = "Phấn rôm tinh chất hoa cúc Chicco 150g chống rôm sảy", Price =  110000m, StockQuantity =  45, CategoryId =  7 },
                new Product { ProductId = 39, Barcode = "893456789039", ProductName = "Sữa dưỡng ẩm toàn thân Cetaphil Baby Daily Lotion 400ml", Price =  235000m, StockQuantity =  12, CategoryId =  7 },
                new Product { ProductId = 40, Barcode = "893456789040", ProductName = "Bàn chải đánh răng xỏ ngón silicon siêu mềm Richell", Price =   45000m, StockQuantity =  90, CategoryId =  8 },
                new Product { ProductId = 41, Barcode = "893456789041", ProductName = "Kem đánh răng hữu cơ nuốt được Azetabio hương táo 50ml", Price =  165000m, StockQuantity =  35, CategoryId =  8 },
                new Product { ProductId = 42, Barcode = "893456789042", ProductName = "Gạc rơ lưỡi tưa lưỡi thảo dược Dr.Papie hộp 30 gói", Price =   95000m, StockQuantity = 130, CategoryId =  8 },
                new Product { ProductId = 43, Barcode = "893456789043", ProductName = "Bộ bao tay bao chân sơ sinh sợi tre Miomio kháng khuẩn", Price =   35000m, StockQuantity = 100, CategoryId =  9 },
                new Product { ProductId = 44, Barcode = "893456789044", ProductName = "Mũ chụp sơ sinh bo viền Nous chất liệu Nu Petit co giãn", Price =   45000m, StockQuantity =  60, CategoryId =  9 },
                new Product { ProductId = 45, Barcode = "893456789045", ProductName = "Gối chặn bông hạt xốp chống giật mình Uala Rogo cho bé", Price =  185000m, StockQuantity =  22, CategoryId = 10 },
                new Product { ProductId = 46, Barcode = "893456789046", ProductName = "Khăn quấn ủ kén sơ sinh SwaddleMe cotton co giãn 4 chiều", Price =  135000m, StockQuantity =  14, CategoryId = 10 },
                new Product { ProductId = 47, Barcode = "893456789047", ProductName = "Xe đẩy du lịch gấp gọn siêu nhẹ Seebaby QQ3 thoáng khí", Price =  890000m, StockQuantity =   6, CategoryId = 12 },
                new Product { ProductId = 48, Barcode = "893456789048", ProductName = "Địu em bé trợ lực 4 tư thế Ergobaby Omni 360 Cool Air", Price = 1250000m, StockQuantity =   5, CategoryId = 12 },
                new Product { ProductId = 49, Barcode = "893456789049", ProductName = "Đai nịt bụng định hình thoáng khí sau sinh Farlin", Price =  290000m, StockQuantity =  18, CategoryId = 14 },
                new Product { ProductId = 50, Barcode = "893456789050", ProductName = "Máy hút sữa điện đôi Spectra 9 Plus có pin tích điện", Price = 2350000m, StockQuantity =   9, CategoryId = 15 }
            );

            // ------------------------------------------------------------------
            // 4. DATA SEEDING: BẢNG CUSTOMERS (15 khách hàng thân thiết)
            // ------------------------------------------------------------------
            modelBuilder.Entity<Customer>().HasData(
                new Customer { CustomerId =  1, CustomerName = "Nguyễn Thị Lan Anh",   PhoneNumber = "0901122334", Address = "12 Lê Lợi, Quận 1, TP.HCM",               MembershipRank = "Vàng",     RewardPoints = 350 },
                new Customer { CustomerId =  2, CustomerName = "Trần Thị Thu Hương",   PhoneNumber = "0918877665", Address = "45 Nguyễn Trãi, Quận 5, TP.HCM",           MembershipRank = "Bạc",      RewardPoints = 120 },
                new Customer { CustomerId =  3, CustomerName = "Lê Thị Bích Ngọc",     PhoneNumber = "0983344556", Address = "78 Cách Mạng Tháng 8, Quận 3, TP.HCM",     MembershipRank = "Chuẩn",    RewardPoints =  25 },
                new Customer { CustomerId =  4, CustomerName = "Phạm Thị Thanh Tâm",   PhoneNumber = "0903456789", Address = "23 Trần Hưng Đạo, Quận 1, TP.HCM",         MembershipRank = "Bạch Kim", RewardPoints = 520 },
                new Customer { CustomerId =  5, CustomerName = "Hoàng Thị Minh Châu",  PhoneNumber = "0905123456", Address = "56 Võ Văn Tần, Quận 3, TP.HCM",            MembershipRank = "Vàng",     RewardPoints = 210 },
                new Customer { CustomerId =  6, CustomerName = "Đỗ Thị Kim Liên",      PhoneNumber = "0912345678", Address = "89 Hai Bà Trưng, Quận 1, TP.HCM",          MembershipRank = "Bạc",      RewardPoints =  85 },
                new Customer { CustomerId =  7, CustomerName = "Vũ Thị Hồng Nhung",    PhoneNumber = "0938765432", Address = "15 Phan Xích Long, Phú Nhuận, TP.HCM",     MembershipRank = "Chuẩn",    RewardPoints =  15 },
                new Customer { CustomerId =  8, CustomerName = "Ngô Thị Mỹ Linh",      PhoneNumber = "0977112233", Address = "102 Lý Thường Kiệt, Quận 10, TP.HCM",      MembershipRank = "Bạch Kim", RewardPoints = 480 },
                new Customer { CustomerId =  9, CustomerName = "Bùi Thị Phương Anh",   PhoneNumber = "0966778899", Address = "34 Nguyễn Văn Cừ, Quận 5, TP.HCM",        MembershipRank = "Vàng",     RewardPoints = 175 },
                new Customer { CustomerId = 10, CustomerName = "Đặng Thị Thanh Mai",   PhoneNumber = "0988001122", Address = "67 Điện Biên Phủ, Bình Thạnh, TP.HCM",    MembershipRank = "Chuẩn",    RewardPoints =  40 },
                new Customer { CustomerId = 11, CustomerName = "Dương Thị Quỳnh Anh",  PhoneNumber = "0945667788", Address = "21 Lạc Long Quân, Quận 11, TP.HCM",        MembershipRank = "Bạc",      RewardPoints =  95 },
                new Customer { CustomerId = 12, CustomerName = "Lý Thị Ngọc Hà",       PhoneNumber = "0933221100", Address = "90 Quang Trung, Gò Vấp, TP.HCM",           MembershipRank = "Vàng",     RewardPoints = 265 },
                new Customer { CustomerId = 13, CustomerName = "Trương Thị Cẩm Vân",   PhoneNumber = "0902334455", Address = "5 Nguyễn Thị Minh Khai, Quận 3, TP.HCM",  MembershipRank = "Chuẩn",    RewardPoints =   0 },
                new Customer { CustomerId = 14, CustomerName = "Phan Thị Thu Hà",      PhoneNumber = "0919889977", Address = "143 Tô Hiến Thành, Quận 10, TP.HCM",       MembershipRank = "Bạch Kim", RewardPoints = 380 },
                new Customer { CustomerId = 15, CustomerName = "Huỳnh Thị Gia Linh",   PhoneNumber = "0987556644", Address = "28 Âu Cơ, Tân Bình, TP.HCM",              MembershipRank = "Bạc",      RewardPoints =  60 }
            );

            // ------------------------------------------------------------------
            // 5. DATA SEEDING: BẢNG ORDERS (15 hóa đơn bán lẻ)
            // ------------------------------------------------------------------
            modelBuilder.Entity<Order>().HasData(
                new Order { OrderId =  1, OrderDate = new DateTime(2026, 10, 1,  8, 30, 0), CustomerId =  1, CashierUsername = "cashier01", TotalAmount =  380000m, PointsEarned = 38 },
                new Order { OrderId =  2, OrderDate = new DateTime(2026, 10, 1,  9, 15, 0), CustomerId =  2, CashierUsername = "cashier01", TotalAmount =  520000m, PointsEarned = 52 },
                new Order { OrderId =  3, OrderDate = new DateTime(2026, 10, 1, 10,  0, 0), CustomerId =  3, CashierUsername = "cashier02", TotalAmount =  270000m, PointsEarned = 27 },
                new Order { OrderId =  4, OrderDate = new DateTime(2026, 10, 2,  8, 45, 0), CustomerId =  4, CashierUsername = "cashier02", TotalAmount =  685000m, PointsEarned = 68 },
                new Order { OrderId =  5, OrderDate = new DateTime(2026, 10, 2, 11, 20, 0), CustomerId =  5, CashierUsername = "cashier03", TotalAmount =  185000m, PointsEarned = 18 },
                new Order { OrderId =  6, OrderDate = new DateTime(2026, 10, 3, 14, 10, 0), CustomerId =  6, CashierUsername = "cashier03", TotalAmount =  450000m, PointsEarned = 45 },
                new Order { OrderId =  7, OrderDate = new DateTime(2026, 10, 3, 16, 40, 0), CustomerId =  7, CashierUsername = "cashier04", TotalAmount =  125000m, PointsEarned = 12 },
                new Order { OrderId =  8, OrderDate = new DateTime(2026, 10, 4,  9,  5, 0), CustomerId =  8, CashierUsername = "cashier04", TotalAmount =  970000m, PointsEarned = 97 },
                new Order { OrderId =  9, OrderDate = new DateTime(2026, 10, 4, 13, 30, 0), CustomerId =  9, CashierUsername = "cashier05", TotalAmount =  330000m, PointsEarned = 33 },
                new Order { OrderId = 10, OrderDate = new DateTime(2026, 10, 5, 10, 50, 0), CustomerId = 10, CashierUsername = "cashier05", TotalAmount =  245000m, PointsEarned = 24 },
                new Order { OrderId = 11, OrderDate = new DateTime(2026, 10, 5, 17, 15, 0), CustomerId = 11, CashierUsername = "cashier06", TotalAmount =  195000m, PointsEarned = 19 },
                new Order { OrderId = 12, OrderDate = new DateTime(2026, 10, 6,  8, 20, 0), CustomerId = 12, CashierUsername = "cashier06", TotalAmount = 1010000m, PointsEarned = 101 },
                new Order { OrderId = 13, OrderDate = new DateTime(2026, 10, 6, 15,  0, 0), CustomerId = 13, CashierUsername = "cashier05", TotalAmount =  280000m, PointsEarned = 28 },
                new Order { OrderId = 14, OrderDate = new DateTime(2026, 10, 7, 12, 10, 0), CustomerId = 14, CashierUsername = "cashier06", TotalAmount =  760000m, PointsEarned = 76 },
                new Order { OrderId = 15, OrderDate = new DateTime(2026, 10, 7, 18, 45, 0), CustomerId = 15, CashierUsername = "cashier01", TotalAmount =  165000m, PointsEarned = 16 }
            );

            // ------------------------------------------------------------------
            // 6. DATA SEEDING: BẢNG ORDERDETAILS (23 dòng chi tiết hóa đơn)
            //    OrderDetail lưu snapshot tên sản phẩm tại thời điểm bán
            // ------------------------------------------------------------------
            modelBuilder.Entity<OrderDetail>().HasData(
                // Đơn 1: Tã Pampers NB + Khăn ướt Bobby + Sữa tắm Pigeon = 332k
                new OrderDetail { OrderDetailId =  1, OrderId =  1, ProductId =  1, Quantity = 1, UnitPrice = 195000m, ProductName = "Tã dán Pampers Newborn size NB – 84 miếng" },
                new OrderDetail { OrderDetailId =  2, OrderId =  1, ProductId =  4, Quantity = 1, UnitPrice =  42000m, ProductName = "Khăn ướt Bobby không mùi hộp 100 tờ" },
                new OrderDetail { OrderDetailId =  3, OrderId =  1, ProductId = 14, Quantity = 1, UnitPrice =  95000m, ProductName = "Sữa tắm gội Pigeon 2 trong 1 200ml" },
                // Đơn 2: Sữa bột NAN 1 = 520k
                new OrderDetail { OrderDetailId =  4, OrderId =  2, ProductId =  5, Quantity = 1, UnitPrice = 520000m, ProductName = "Sữa bột NAN Optipro 1 hộp 800g" },
                // Đơn 3: 3 Váng sữa Yoplait + 1 Bột ăn dặm Heinz
                new OrderDetail { OrderDetailId =  5, OrderId =  3, ProductId = 11, Quantity = 3, UnitPrice =  65000m, ProductName = "Váng sữa Petit Suisse Yoplait vị dâu lốc 6 hộp" },
                new OrderDetail { OrderDetailId =  6, OrderId =  3, ProductId =  9, Quantity = 1, UnitPrice =  85000m, ProductName = "Bột ăn dặm Heinz vị gạo sữa 200g" },
                // Đơn 4: Sữa Enfamil A+ + Tã Huggies M
                new OrderDetail { OrderDetailId =  7, OrderId =  4, ProductId =  6, Quantity = 1, UnitPrice = 490000m, ProductName = "Sữa bột Enfamil A+ số 2 hộp 900g" },
                new OrderDetail { OrderDetailId =  8, OrderId =  4, ProductId =  2, Quantity = 1, UnitPrice = 185000m, ProductName = "Tã quần Huggies Dry size M – 48 miếng" },
                // Đơn 5: Bình sữa Pigeon PPSU
                new OrderDetail { OrderDetailId =  9, OrderId =  5, ProductId = 12, Quantity = 1, UnitPrice = 185000m, ProductName = "Bình sữa Pigeon PPSU cổ rộng 160ml" },
                // Đơn 6: Sữa bầu Enfamama
                new OrderDetail { OrderDetailId = 10, OrderId =  6, ProductId = 18, Quantity = 1, UnitPrice = 450000m, ProductName = "Sữa bầu Enfamama A+ hộp 900g" },
                // Đơn 7: Kem chống hăm Bepanthen
                new OrderDetail { OrderDetailId = 11, OrderId =  7, ProductId = 15, Quantity = 1, UnitPrice = 125000m, ProductName = "Kem chống hăm Bepanthen Baby 30g" },
                // Đơn 8: Sữa Aptamil 3 + Sữa Dielac
                new OrderDetail { OrderDetailId = 12, OrderId =  8, ProductId =  7, Quantity = 1, UnitPrice = 480000m, ProductName = "Sữa bột Aptamil số 3 hộp 800g" },
                new OrderDetail { OrderDetailId = 13, OrderId =  8, ProductId =  8, Quantity = 1, UnitPrice = 320000m, ProductName = "Sữa bột Dielac Alpha Step 1 hộp 900g" },
                // Đơn 9: 2 Khăn ướt + 1 Bộ quần áo sơ sinh
                new OrderDetail { OrderDetailId = 14, OrderId =  9, ProductId =  4, Quantity = 2, UnitPrice =  42000m, ProductName = "Khăn ướt Bobby không mùi hộp 100 tờ" },
                new OrderDetail { OrderDetailId = 15, OrderId =  9, ProductId = 17, Quantity = 1, UnitPrice = 280000m, ProductName = "Bộ bodysuit cotton sơ sinh 5 bộ size 0-3 tháng" },
                // Đơn 10: 3 Cháo ăn dặm + 2 Váng sữa
                new OrderDetail { OrderDetailId = 16, OrderId = 10, ProductId = 10, Quantity = 3, UnitPrice =  38000m, ProductName = "Cháo tươi ăn dặm Nutricare gà rau củ 200g" },
                new OrderDetail { OrderDetailId = 17, OrderId = 10, ProductId = 11, Quantity = 2, UnitPrice =  65000m, ProductName = "Váng sữa Petit Suisse Yoplait vị dâu lốc 6 hộp" },
                // Đơn 12: Sữa NAN 1 + Tã Bobby S + Bình sữa Dr. Brown
                new OrderDetail { OrderDetailId = 18, OrderId = 12, ProductId =  5, Quantity = 1, UnitPrice = 520000m, ProductName = "Sữa bột NAN Optipro 1 hộp 800g" },
                new OrderDetail { OrderDetailId = 19, OrderId = 12, ProductId =  3, Quantity = 1, UnitPrice = 135000m, ProductName = "Tã dán Bobby Newborn size S – 56 miếng" },
                new OrderDetail { OrderDetailId = 20, OrderId = 12, ProductId = 13, Quantity = 1, UnitPrice = 245000m, ProductName = "Bình sữa Dr. Brown's chống sặc 250ml" },
                // Đơn 14: Sữa bầu + Vitamin mẹ bầu
                new OrderDetail { OrderDetailId = 21, OrderId = 14, ProductId = 18, Quantity = 1, UnitPrice = 450000m, ProductName = "Sữa bầu Enfamama A+ hộp 900g" },
                new OrderDetail { OrderDetailId = 22, OrderId = 14, ProductId = 19, Quantity = 1, UnitPrice = 380000m, ProductName = "Vitamin Blackmores Pregnancy & Breast-feeding 60 viên" },
                // Đơn 15: Bộ đồ chơi xúc xắc
                new OrderDetail { OrderDetailId = 23, OrderId = 15, ProductId = 20, Quantity = 1, UnitPrice = 165000m, ProductName = "Bộ đồ chơi xúc xắc gặm nướu an toàn cho bé 8 món" }
            );
        }
    }
}
