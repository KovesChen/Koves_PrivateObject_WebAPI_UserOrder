using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Koves.UserOrder.WebApi.Models
{
    public class UserRoles
    {

        public int UserId { get; set; }
        public Users Users { get; set; }   // 父：Users

        public int RoleId { get; set; }
        public Roles Roles { get; set; }   // 父：Role
    }
}
