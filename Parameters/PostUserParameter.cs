using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Services;
using Koves.UserOrder.WebApi.Utilities;
using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Parameters
{
    /**
     * 新增/修改使用者用
     */
    public class PostUserParameter
    {
        [Required(ErrorMessage = "Account is required")]
        [StringLength(15, MinimumLength = 6)]
        public string Account { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(15, MinimumLength = 9)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
