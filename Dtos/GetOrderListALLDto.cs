namespace Koves.UserOrder.WebApi.Dtos
{
    public class GetOrderListALLDto
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public int Creater { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<GetOrderItemListALLDto> OrderItems { get; set; } = new();
    }
}
