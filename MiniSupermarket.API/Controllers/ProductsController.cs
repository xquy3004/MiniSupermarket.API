using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSupermarket.API.Data;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Phải đăng nhập mới xem được (POS của Cashier cũng cần đọc sản phẩm)
    public class ProductsController : ControllerBase
    {
        private readonly SupermarketDbContext _context;

        public ProductsController(SupermarketDbContext context)
        {
            _context = context;
        }

        // Truy vấn dùng chung: trả về dữ liệu gọn kèm tên nhóm hàng
        private IQueryable<ProductResponse> ProductQuery()
        {
            return _context.Products.AsNoTracking().Select(p => new ProductResponse
            {
                ProductId = p.ProductId,
                Barcode = p.Barcode,
                ProductName = p.ProductName,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                CategoryId = p.CategoryId,
                CategoryName = p.Category != null ? p.Category.CategoryName : string.Empty
            });
        }

        // =========================================================================
        // BUỔI 6: PHÂN TRANG LINQ & TÌM KIẾM SẢN PHẨM (EAGER LOADING)
        // =========================================================================
        // GET: api/products/paged?keyword=sua&categoryId=1&pageNumber=1&pageSize=5
        [HttpGet("paged")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPagedProducts(
            [FromQuery] string? keyword,
            [FromQuery] int? categoryId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            // 1. Kiểm tra tham số trang đầu vào hợp lệ
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 50) pageSize = 10;

            // 2. Khởi tạo truy vấn LINQ IQueryable kết hợp Eager Loading (.Include)
            var query = _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .AsQueryable();

            // 3. Lọc theo từ khóa tìm kiếm (nếu người dùng có nhập)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string search = keyword.Trim().ToLower();
                query = query.Where(p => p.ProductName.ToLower().Contains(search)
                                      || p.Barcode.Contains(search));
            }

            // BÀI TẬP 6.2: Lọc theo nhóm hàng (categoryId)
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            // 4. Đếm tổng số lượng bản ghi thỏa mãn điều kiện
            int totalItems = await query.CountAsync();

            // 5. Tính toán tổng số trang
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            // 6. Áp dụng toán tử Skip và Take để phân trang trực tiếp dưới SQL Server
            var items = await query
                .OrderBy(p => p.ProductId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new ProductItemDto
                {
                    ProductId = p.ProductId,
                    Barcode = p.Barcode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : string.Empty,
                    IsLowStock = p.StockQuantity <= 20
                })
                .ToListAsync();

            // 7. Đóng gói kết quả trả về cho Client
            var response = new PagedResult<ProductItemDto>
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Items = items
            };

            return Ok(response);
        }

        // =========================================================================
        // PHỤ LỤC - BÀI TẬP 1: LẤY DANH SÁCH KÈM TÊN DANH MỤC (EAGER LOADING)
        // =========================================================================
        // GET: api/products/with-category
        [HttpGet("with-category")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsWithCategory()
        {
            var list = await _context.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .Select(p => new ProductItemDto
                {
                    ProductId = p.ProductId,
                    Barcode = p.Barcode,
                    ProductName = p.ProductName,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.CategoryName : string.Empty,
                    IsLowStock = p.StockQuantity <= 20
                })
                .OrderBy(p => p.ProductId)
                .ToListAsync();

            return Ok(list);
        }

        // 1. GET /api/products?barcode=...&categoryId=...  (cả 2 tham số đều không bắt buộc)
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? barcode, [FromQuery] int? categoryId)
        {
            var query = ProductQuery();

            if (!string.IsNullOrWhiteSpace(barcode))
            {
                string key = barcode.Trim();
                query = query.Where(p => p.Barcode.Contains(key));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
            }

            var list = await query.OrderBy(p => p.ProductId).ToListAsync();
            return Ok(list);
        }

        // 2. GET /api/products/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await ProductQuery().FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm!" });
            }
            return Ok(product);
        }

        // 3. GET /api/products/barcode/8930000000011  (dùng cho màn hình POS ở Phần D)
        [HttpGet("barcode/{barcode}")]
        public async Task<IActionResult> GetByBarcode(string barcode)
        {
            var product = await ProductQuery().FirstOrDefaultAsync(p => p.Barcode == barcode);
            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm có mã vạch này!" });
            }
            return Ok(product);
        }

        // 4. POST /api/products  (chỉ Admin và Warehouse)
        [HttpPost]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Create([FromBody] Product newProd)
        {
            if (newProd.Price < 0 || newProd.StockQuantity < 0)
            {
                return BadRequest(new { message = "Đơn giá và tồn kho không được âm!" });
            }

            if (!await _context.Categories.AnyAsync(c => c.CategoryId == newProd.CategoryId))
            {
                return BadRequest(new { message = "Nhóm hàng không tồn tại!" });
            }

            if (await _context.Products.AnyAsync(p => p.Barcode == newProd.Barcode))
            {
                return BadRequest(new { message = "Mã vạch này đã tồn tại!" });
            }

            newProd.ProductId = 0;
            newProd.Category = null;
            _context.Products.Add(newProd);
            await _context.SaveChangesAsync();

            var created = await ProductQuery().FirstAsync(p => p.ProductId == newProd.ProductId);
            return CreatedAtAction(nameof(GetById), new { id = newProd.ProductId }, created);
        }

        // 5. PUT /api/products/5  (chỉ Admin và Warehouse)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Update(int id, [FromBody] Product updateProd)
        {
            var prod = await _context.Products.FindAsync(id);
            if (prod == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm cần sửa!" });
            }

            if (updateProd.Price < 0 || updateProd.StockQuantity < 0)
            {
                return BadRequest(new { message = "Đơn giá và tồn kho không được âm!" });
            }

            if (!await _context.Categories.AnyAsync(c => c.CategoryId == updateProd.CategoryId))
            {
                return BadRequest(new { message = "Nhóm hàng không tồn tại!" });
            }

            if (await _context.Products.AnyAsync(p => p.Barcode == updateProd.Barcode && p.ProductId != id))
            {
                return BadRequest(new { message = "Mã vạch này đã được sản phẩm khác sử dụng!" });
            }

            prod.Barcode = updateProd.Barcode;
            prod.ProductName = updateProd.ProductName;
            prod.Price = updateProd.Price;
            prod.StockQuantity = updateProd.StockQuantity;
            prod.CategoryId = updateProd.CategoryId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 6. DELETE /api/products/5  (chỉ Admin và Warehouse)
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin,Warehouse")]
        public async Task<IActionResult> Delete(int id)
        {
            var prod = await _context.Products.FindAsync(id);
            if (prod == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm cần xóa!" });
            }
            // Sản phẩm đã nằm trong hóa đơn thì không xóa được
            if (await _context.OrderDetails.AnyAsync(d => d.ProductId == id))
            {
                return BadRequest(new { message = "Sản phẩm này đã có trong hóa đơn nên không thể xóa! Hãy đặt tồn kho về 0 nếu không bán nữa." });
            }

            _context.Products.Remove(prod);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    // Dữ liệu trả về cho client (tránh trả thẳng entity)
    public class ProductResponse
    {
        public int ProductId { get; set; }
        public string Barcode { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}