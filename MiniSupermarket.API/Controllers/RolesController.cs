using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MiniSupermarket.API.Models;

namespace MiniSupermarket.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RolesController : ControllerBase
    {
        // Dữ liệu mẫu lưu tạm trên RAM phục vụ kiểm thử (In-Memory theo đề Buổi 1)
        private static readonly List<Role> _roles = new()
        {
            new Role { Id = 1, RoleName = "Admin", Description = "Quản trị viên toàn hệ thống, toàn quyền thao tác" },
            new Role { Id = 2, RoleName = "Cashier", Description = "Nhân viên thu ngân, chuyên trách bán hàng tại quầy POS" },
            new Role { Id = 3, RoleName = "Warehouse", Description = "Nhân viên quản lý kho, nhập hàng và kiểm kê" }
        };

        // 1. READ: Lấy toàn bộ danh sách vai trò (GET /api/roles)
        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_roles);
        }

        // 2. READ: Lấy chi tiết một vai trò theo ID (GET /api/roles/{id})
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound(new { message = "Không tìm thấy vai trò!" });
            }
            return Ok(role);
        }

        // 3. SEARCH: Tìm kiếm vai trò theo tên (GET /api/roles/search?keyword=...)
        [HttpGet("search")]
        public IActionResult Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Vui lòng nhập từ khóa!" });
            }

            var results = _roles.Where(r =>
                r.RoleName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                (r.Description != null && r.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            ).ToList();

            return Ok(results);
        }

        // 4. CREATE: Thêm mới vai trò (POST /api/roles)
        [HttpPost]
        public IActionResult Create([FromBody] Role newRole)
        {
            if (string.IsNullOrWhiteSpace(newRole.RoleName))
            {
                return BadRequest(new { message = "Tên vai trò không được để trống!" });
            }

            // Tự động tăng ID
            newRole.Id = _roles.Count > 0 ? _roles.Max(r => r.Id) + 1 : 1;
            _roles.Add(newRole);

            return CreatedAtAction(nameof(GetById), new { id = newRole.Id }, newRole);
        }

        // 5. UPDATE: Cập nhật thông tin vai trò (PUT /api/roles/{id})
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Role updateRole)
        {
            if (string.IsNullOrWhiteSpace(updateRole.RoleName))
            {
                return BadRequest(new { message = "Tên vai trò không được để trống!" });
            }

            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound(new { message = "Không tìm thấy vai trò cần sửa!" });
            }

            role.RoleName = updateRole.RoleName;
            role.Description = updateRole.Description;

            return NoContent();
        }

        // 6. DELETE: Xóa vai trò theo ID (DELETE /api/roles/{id})
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var role = _roles.FirstOrDefault(r => r.Id == id);
            if (role == null)
            {
                return NotFound(new { message = "Không tìm thấy vai trò cần xóa!" });
            }

            _roles.Remove(role);
            return NoContent();
        }
    }
}
