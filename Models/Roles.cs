using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Models
{
    public class Roles
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        // 對照子表 (一個 Role 對應多個 UserRole)
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
