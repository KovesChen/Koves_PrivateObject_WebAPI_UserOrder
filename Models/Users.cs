using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Models
{
    public class Users
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }

        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Active { get; set; }

        public int Creater { get; set; }

        public DateTime CreatedAt { get; set; }

        // 對照子表 (一個 User 對應多個 UserRole)
        public ICollection<UserRoles> UserRoles { get; set; }

        // 對照子表 (一個 User 對應多個 Order)
        public ICollection<Orders> Orders { get; set; }
    }
}
