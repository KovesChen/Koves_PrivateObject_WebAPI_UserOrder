using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Models
{
    public class OrderItems
    {
        [Key]
        public int ItemNo { get; set; }

        public int OrderId { get; set; }
        public Orders Orders { get; set; }   // 父：Orders

        public int ProductId { get; set; }
        public Products Products { get; set; }   // 父：Products

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotPrice { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
