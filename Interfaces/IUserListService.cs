using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Parameters;

namespace Koves.UserOrder.WebApi.Interfaces
{
    public interface IUserListService
    {
        int LoginUserID { get;}
        string LoginUserRoles { get;}

        Task<IEnumerable<GetUserDto>> GetUserAsync(GetUserListParameter Values);
        Task<IEnumerable<GetUserListALLDto>> GetUserALLAsync(GetUserListParameter Values);

        Task<IEnumerable<GetUserListALLDto>> GetUserByAccountPasswordAsync(LoginPostParamenter Values);

        Task<IEnumerable<GetUserRolesDto>> GetUserRolesAsync(GetUserListParameter Values);

        Task<IEnumerable<GetUserListALLDto>> AddUserAsync(PostUserParameter values);

        Task<IEnumerable<GetUserListALLDto>> UpdUserAsync(UpdUserParamenter Values);

        Task<IEnumerable<GetUserDto>> UpdUserPassWordAsync(UpdUserParamenter Values);

        Task<bool> UpdUserActiveAsync(DelUserParamenter Values);

        Task<bool> DelUserAsync(DelUserParamenter Values);
    }
}
