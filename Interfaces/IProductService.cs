using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Parameters;

namespace Koves.UserOrder.WebApi.Interfaces
{
    public interface IProductService
    {
        int LoginUserID { get; }
        string LoginUserRoles { get; }
        Task<IEnumerable<GetProductListDto>> GetProductListAsync(GetProductListParamenter Values);

        Task<IEnumerable<GetProductListALLInfoDto>> GetProductListAllInfoAsync(GetProductListParamenter Values);

        Task<IEnumerable<GetProductListALLInfoDto>> AddProductAsync(PostProductParamenter Values);

        Task<IEnumerable<GetProductListALLInfoDto>> UpdProductAsync(updProductParamenter Values);

        Task<bool> DelProductAsync(DelProductParamenter Values);
    }
}
