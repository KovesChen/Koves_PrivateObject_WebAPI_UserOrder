using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Models
{
    public class Products
    {
        [Key]
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string Active { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public int Creater { get; set; }
        public DateTime CreatedAt { get; set; }

        // 對照子表 (一個 Products 對應多個 OrderItems)
        public ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }
}
