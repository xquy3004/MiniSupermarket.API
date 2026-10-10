using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniSupermarket.API.Models
{
    // Hóa đơn bán hàng (một lần thanh toán tại quầy POS)
    [Table("Orders")]
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Tên đăng nhập của thu ngân lập hóa đơn (lấy từ JWT, không tin client)
        [Required]
        [StringLength(50)]
        public string CashierUsername { get; set; } = string.Empty;

        // null = khách lẻ, không tích điểm
        public int? CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Điểm thưởng cộng cho khách trong hóa đơn này
        public int PointsEarned { get; set; }

        public virtual ICollection<OrderDetail> Details { get; set; } = new List<OrderDetail>();
    }
}