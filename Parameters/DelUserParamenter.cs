using Koves.UserOrder.WebApi.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Parameters
{
    public class DelUserParamenter
    {
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }

    }
}
