using Koves.UserOrder.WebApi.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Parameters
{
    public class PostOrderParamenter
    {
        [Required(ErrorMessage = "UserId is required")]
        public int UserId { get; set; }

        // 商品清單 
        public List<OrderProductParamenter> ProductList { get; set; } = new List<OrderProductParamenter>();

    }
    public class OrderProductParamenter
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "數量需大於 0")]
        public int Quantity { get; set; }        
    }
}
