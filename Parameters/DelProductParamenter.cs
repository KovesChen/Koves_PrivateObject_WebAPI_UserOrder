using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Parameters
{
    public class DelProductParamenter
    {
        [Required(ErrorMessage = "ProductId is required")]
        public int ProductId { get; set; }

    }
}
