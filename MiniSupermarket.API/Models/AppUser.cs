using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSupermarket.API.Models
{
    [Table("Users")]
    public class AppUser
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        // Chỉ lưu bản băm của mật khẩu, không bao giờ lưu mật khẩu gốc
        [Required]
        [StringLength(200)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        // Admin / Cashier / Warehouse
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "Cashier";

        // false = tài khoản bị khóa
        public bool IsActive { get; set; } = true;
    }
}