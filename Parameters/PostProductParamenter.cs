using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Parameters
{
    public class PostProductParamenter
    {
        [Required(ErrorMessage = "ProductName is required")]
        [StringLength(200, MinimumLength = 6)]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Price is required")]
        [Range(1, int.MaxValue, ErrorMessage = "價格需大於 0")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Stock is required")]
        [Range(1, 100, ErrorMessage = "庫存量需大於 0")]
        public int Stock { get; set; }

    }
}
