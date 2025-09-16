namespace Koves.UserOrder.WebApi.Dtos
{
    public class GetProductListALLInfoDto : GetProductListDto
    {

        public string Active { get; set; }

        public int Creater { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
