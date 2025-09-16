using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Models;
using Koves.UserOrder.WebApi.Parameters;
using Koves.UserOrder.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

/**
 * Koves 開發專案_JWT身分認證Controller
 */
namespace Koves.UserOrder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IUserListService _userListService;

        public LoginController(IConfiguration configuration, IUserListService userListService)
        {
            _configuration = configuration;
            _userListService = userListService;
        }  

        // JWT 登入認證
        [HttpPost]
        public async Task<IActionResult> jwtLogin(LoginPostParamenter value)
        {
            // 確認帳號是否存在
            var users = await _userListService.GetUserByAccountPasswordAsync(value);
            var user  = users.FirstOrDefault();

            if (user == null)
            {
                return BadRequest("帳號密碼錯誤");
            }
            try
            {
                //驗證
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim("Email", user.Email),
                    new Claim(JwtRegisteredClaimNames.NameId, Convert.ToString(user.Id)),
                };
                var Roles = await _userListService.GetUserRolesAsync(new GetUserListParameter { Id = user.Id });
                // 設定帳號標籤
                foreach (var item in Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, item.RoleName));
                }

                //取出appsettings.json裡的KEY處理
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));
                //設定jwt相關資訊
                var jwt = new JwtSecurityToken
                (
                    issuer: _configuration["JWT:Issuer"],   // 發行者
                    audience: _configuration["JWT:Audience"],  //使用者
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),  // 有效期限 10分鐘
                    signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
                );

                //產生JWT Token
                var token = new JwtSecurityTokenHandler().WriteToken(jwt);

                //回傳JWT Token給認證通過的使用者
                return Ok(ResponseDto<string>.SuccessResponse(token, "認證成功"));
            }
            catch (Exception ex){
                string Message = $"帳號 {value.Account} 登入失敗";
                string UserRole = "User";
                if (StringHelper.StringEqualsIgnoreSpace(value.Account, "Administrator"))
                {
                    UserRole = "Admin";
                }
                return BadRequest(ResponseDto<object>.FailResponse(UserRole,
                                    Message, ex));
            }
        }

    }
}
