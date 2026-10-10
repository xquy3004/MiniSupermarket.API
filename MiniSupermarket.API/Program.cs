using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Helpers;
using MiniSupermarket.API.Models;


var builder = WebApplication.CreateBuilder(args);
// Lấy chuỗi kết nối từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Đăng ký DbContext sử dụng SQL Server qua cơ chế Dependency Injection (DI)
builder.Services.AddDbContext<SupermarketDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // Bắt buộc phải có dòng này


var jwtSecret = builder.Configuration["JwtSettings:Secret"]
    ?? "SupermarketSecretKeyDoAnMonHoc2026SecureString!!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();
// Nạp sẵn tài khoản mẫu: 15 tài khoản theo đề Buổi 4 + 2 tài khoản cũ của Buổi 2
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SupermarketDbContext>();

    if (!db.Users.Any())
    {
        var seed = new (string Username, string FullName, string Role)[]
        {
            ("admin01", "Nguyễn Quản Trị", "Admin"),
            ("admin02", "Trần Giám Đốc", "Admin"),
            ("cashier01", "Lê Thu Ngân", "Cashier"),
            ("cashier02", "Phạm Bán Hàng", "Cashier"),
            ("cashier03", "Hoàng Thu Ngân", "Cashier"),
            ("cashier04", "Vũ Thị Quầy", "Cashier"),
            ("cashier05", "Đỗ Bán Lẻ", "Cashier"),
            ("ware01", "Ngô Quản Kho", "Warehouse"),
            ("ware02", "Bùi Kiểm Kê", "Warehouse"),
            ("ware03", "Dương Thủ Kho", "Warehouse"),
            ("ware04", "Lý Nhập Hàng", "Warehouse"),
            ("admin_backup", "Đặng Hỗ Trợ", "Admin"),
            ("cashier06", "Hồ Ca Chiều", "Cashier"),
            ("ware05", "Trương Vận Chuyển", "Warehouse"),
            ("supervisor", "Mai Giám Sát", "Admin"),
            // 2 tài khoản cũ của Buổi 2 (để test Swagger)
            ("admin", "Quản trị (Buổi 2)", "Admin"),
            ("cashier", "Thu ngân (Buổi 2)", "Cashier")
        };

        foreach (var s in seed)
        {
            db.Users.Add(new AppUser
            {
                Username = s.Username,
                FullName = s.FullName,
                Role = s.Role,
                PasswordHash = PasswordHelper.Hash("123456"),
                IsActive = true
            });
        }

        db.SaveChanges();
    }

    // Nạp sẵn sản phẩm mẫu để màn hình POS có hàng để bán (chỉ khi bảng Products đang trống)
    if (!db.Products.Any())
    {
        var sampleProducts = new (string Barcode, string Name, decimal Price, int Stock, int CategoryId)[]
        {
            ("8930000000011", "Bánh quy bơ Cosy 150g", 22000m, 100, 1),
            ("8930000000028", "Snack khoai tây Oishi 40g", 8000m, 150, 1),
            ("8930000000035", "Nước suối Aquafina 500ml", 6000m, 200, 2),
            ("8930000000042", "Trà xanh Không Độ 455ml", 10000m, 120, 2),
            ("8930000000059", "Sữa tươi Vinamilk 1L", 32000m, 80, 3),
            ("8930000000066", "Sữa chua Vinamilk hộp 100g", 7000m, 90, 3),
            ("8930000000073", "Mì Hảo Hảo tôm chua cay", 5000m, 300, 4),
            ("8930000000080", "Phở bò ăn liền Vifon", 12000m, 100, 4),
            ("8930000000097", "Nước mắm Nam Ngư 500ml", 38000m, 60, 5),
            ("8930000000103", "Dầu ăn Neptune 1L", 52000m, 50, 5)
        };

        foreach (var p in sampleProducts)
        {
            db.Products.Add(new Product
            {
                Barcode = p.Barcode,
                ProductName = p.Name,
                Price = p.Price,
                StockQuantity = p.Stock,
                CategoryId = p.CategoryId
            });
        }

        db.SaveChanges();
    }
}

// Cấu hình sử dụng Swagger ở môi trường phát triển
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Bắt buộc phải có dòng này để bật giao diện web
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();