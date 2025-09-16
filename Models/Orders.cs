using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Models
{
    public class Orders
    {
        [Key]
        public int OrderId { get; set; }

        public int UserId { get; set; }
        public Users Users { get; set; }   // 父：Users

        public decimal TotalAmount { get; set; }

        public string Status { get; set; }
        public int Creater { get; set; }
        public DateTime CreatedAt { get; set; }

        // 對照子表 (一個 Order 對應多個 OrderItems)
        public ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }
}
