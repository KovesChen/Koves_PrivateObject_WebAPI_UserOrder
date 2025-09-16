using Koves.UserOrder.WebApi.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Parameters
{
    public class LoginPostParamenter
    {
        [Required(ErrorMessage = "Account is required")]
        public string Account { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

    }
}
