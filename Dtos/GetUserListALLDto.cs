namespace Koves.UserOrder.WebApi.Dtos
{
    public class GetUserListALLDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }

        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public string Active { get; set; }

        public int Creater { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
