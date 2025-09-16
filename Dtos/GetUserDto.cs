

using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Dtos
{
    public class GetUserDto
    {
        public int Id { get; set; }
        public string Account { get; set; }

        //public string PasswordHash { get; set; }
        public string Email { get; set; }
        //public string Active { get; set; }

        //public int Creater { get; set; }

        //public DateTime CreatedAt { get; set; }
    }
}
