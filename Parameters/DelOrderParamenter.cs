using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Models;
using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Parameters
{
    public class DelOrderParamenter
    {
        [Required]
        public int OrderId { get; set; }        
    }
}
