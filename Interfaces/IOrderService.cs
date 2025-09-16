using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Parameters;

namespace Koves.UserOrder.WebApi.Interfaces
{
    public interface IOrderService
    {
        int LoginUserID { get; }
        string LoginUserRoles { get; }
        Task<IEnumerable<GetOrderListDto>> GetOrderListAsync(GetOrderListParamenter Values);

        Task<IEnumerable<GetOrderListALLDto>> GetOrderListAllInfoAsync(GetOrderListParamenter Values);

        Task<IEnumerable<GetOrderListALLDto>> AddOrderAsync(PostOrderParamenter Values);

        Task<IEnumerable<GetOrderListALLDto>> UpdOrderStatusAsync(UpdOrderParamenter Values);

        Task<bool> DelOrderAsync(DelOrderParamenter Values);
    }
}
