using AutoMapper;
using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Models;
using Koves.UserOrder.WebApi.Parameters;
using Koves.UserOrder.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
/**
 * Koves 開發專案_User管理 Controller
 */
namespace Koves.UserOrder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly KCdbContext _kdbContext;
        private readonly IMapper _mapper;
        private readonly IUserListService _userListService;

        public UsersController(KCdbContext kdbContext, IMapper mapper,
                                IUserListService userListService)
        {
            _kdbContext = kdbContext;
            _mapper = mapper;
            _userListService = userListService;
        }
        protected int UserID => _userListService.LoginUserID;
        protected string UserRole => _userListService.LoginUserRoles;

        /*
         * 查詢使用者清單
         * */
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetUesrs([FromQuery] GetUserListParameter Values)
        {
            try
            {
                if (StringHelper.StringEqualsIgnoreSpace(this.UserRole, "Admin"))
                {
                    var totUserList = await _userListService.GetUserALLAsync(Values);
                    return Ok(ResponseDto<IEnumerable<GetUserListALLDto>>.SuccessResponse(totUserList, "取得成功"));
                }
                else
                {
                    Values.Id = this.UserID;   // 不是Admin 就預設只能撈自己的資料
                    var UserList = await _userListService.GetUserAsync(Values);
                    return Ok(ResponseDto<IEnumerable<GetUserDto>>.SuccessResponse(UserList, "取得成功"));
                }
            }
            catch (Exception ex)
            {
                string Message = $"帳號 {Values.Id} 取得失敗";
                return BadRequest(ResponseDto<object>.FailResponse(this.UserRole,
                                    Message, ex));
            }
        }
        /**
         * 註冊帳號
         */
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddUser([FromBody] PostUserParameter Values)
        {
            try
            {
                var result = await _userListService.AddUserAsync(Values);
                var Result = _mapper.Map<IEnumerable<GetUserDto>>(result);

                return Ok(ResponseDto<IEnumerable<GetUserDto>>.SuccessResponse(Result, "新增成功"));
            }
            catch (Exception ex)
            {
                string Message = $"帳號 {Values.Account} 資料新增失敗";
                return BadRequest(ResponseDto<object>.FailResponse(this.UserRole,
                                    Message, ex));
            }
        }
        [HttpPatch]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdUser([FromBody] UpdUserParamenter Values)
        {
            try
            {
                if (StringHelper.StringEqualsIgnoreSpace(this.UserRole, "Admin"))
                {
                    var updAll = await _userListService.UpdUserAsync(Values);
                    return Ok(ResponseDto<IEnumerable<GetUserListALLDto>>.SuccessResponse(updAll, "更新成功"));
                }
                else
                {
                    //一般使用者只能改自己的帳號
                    if (this.UserID != Values.Id)
                    {
                        return Forbid();
                    }
                    var updPasssword = await _userListService.UpdUserPassWordAsync(Values);
                    return Ok(ResponseDto<IEnumerable<GetUserDto>>.SuccessResponse(updPasssword, "更新成功"));
                }
            }
            catch (Exception ex)
            {
                string Message = $"帳號 {Values.Account} 資料更新失敗";
                return BadRequest(ResponseDto<object>.FailResponse(this.UserRole,
                                    Message, ex));
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> DelUser([FromQuery] DelUserParamenter Values)
        {
            try
            {
                bool result = false;
                if (StringHelper.StringEqualsIgnoreSpace(this.UserRole, "Admin"))
                {
                    result = await _userListService.DelUserAsync(Values);
                }
                else
                {
                    //一般使用者只能失效自己的帳號(非完全刪除)
                    if (this.UserID != Values.Id)
                    {
                        return Forbid();
                    }
                    result = await _userListService.UpdUserActiveAsync(Values);
                }
                string Message = "帳號 =>" + Values.Id + (result ? "刪除成功" : "刪除失敗");
                return Ok(ResponseDto<bool>.SuccessResponse(result, Message));
            }
            catch (Exception ex)
            {
                string Message = $"帳號 {Values.Id} 刪除失敗";
                return BadRequest(ResponseDto<object>.FailResponse(this.UserRole,
                                    Message, ex));
            }
        }

    }
}
