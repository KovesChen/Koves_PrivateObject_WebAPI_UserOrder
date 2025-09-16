using Koves.UserOrder.WebApi.Models;

namespace Koves.UserOrder.WebApi.Dtos
{
    public class GetOrderItemListDto
    {
        public int ItemNo { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotPrice { get; set; }        
    }
}
