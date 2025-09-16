using Koves.UserOrder.WebApi.Models;

namespace Koves.UserOrder.WebApi.Dtos
{
    public class GetOrderListDto
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; }

        public List<GetOrderItemListDto> OrderItems { get; set; } = new();
    }
}
