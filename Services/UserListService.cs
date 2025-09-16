using AutoMapper;
using AutoMapper.QueryableExtensions;
using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Models;
using Koves.UserOrder.WebApi.Parameters;
using Koves.UserOrder.WebApi.Profiles;
using Koves.UserOrder.WebApi.Utilities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Koves.UserOrder.WebApi.Services
{
    public class UserListService : IUserListService
    {

        private readonly KCdbContext _kdbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserListService(KCdbContext kdbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _kdbContext = kdbContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        //取得登入者資訊 ID / Roles  
        public int LoginUserID
        {
            get
            {
                var User = _httpContextAccessor.HttpContext?.User;
                var idClaim = User?.FindFirst(ClaimTypes.NameIdentifier);
                return idClaim != null && int.TryParse(idClaim.Value, out var id) ? id : 0;
            }
        }
        public string LoginUserRoles
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role);
                return userIdClaim != null ? userIdClaim.Value : "User";  //沒資料預設User
            }
        }



        //取得User基本資料(回傳用)
        public async Task<IEnumerable<GetUserDto>> GetUserAsync(GetUserListParameter Values)
        {
            var result = await GetUserALLAsync(Values);

            return _mapper.Map<IEnumerable<GetUserDto>>(result);
        }
        // 取得User全部資料
        public async Task<IEnumerable<GetUserListALLDto>> GetUserALLAsync(GetUserListParameter Values)
        {
            var result = _kdbContext.Users.AsQueryable();

            if (Values.Id != null)
            {
                result = result.Where(a => a.Id == Values.Id);
            }
            // User 不能查失效的資料
            if (!StringHelper.StringEqualsIgnoreSpace(this.LoginUserRoles, "Admin"))
            {
                result = result.Where(a => a.Active != "D");
            }
            if (Values.Account != null || Values.Email != null)
            {
                result = result.Where(a =>
                    (Values.Account != null && a.UserName == Values.Account) ||
                    (Values.Email != null && a.Email == Values.Email));
            }

            return await result.ProjectTo<GetUserListALLDto>(_mapper.ConfigurationProvider).ToListAsync();
        }
        // 登入用,確認登入資訊是否正確
        public async Task<IEnumerable<GetUserListALLDto>> GetUserByAccountPasswordAsync(LoginPostParamenter Values)
        {
            var user = await _kdbContext.Users
                .FirstOrDefaultAsync(u => u.UserName == Values.Account && u.PasswordHash == Values.Password);

            if (user == null)
            {
                return Enumerable.Empty<GetUserListALLDto>();
            }
            return _mapper.Map<IEnumerable<GetUserListALLDto>>(new[] { user });
        }
        // 撈取User 身分設定檔
        public async Task<IEnumerable<GetUserRolesDto>> GetUserRolesAsync(GetUserListParameter Values)
        {
            var Roles = await _kdbContext.Users
                .Where(u => u.Id == Values.Id)
                .SelectMany(u => u.UserRoles)
                .Select(ur => new GetUserRolesDto
                {
                    UserId = ur.UserId,
                    RoleId = ur.Roles.RoleId,
                    RoleName = ur.Roles.RoleName
                }).ToListAsync(); ;

            return Roles;
        }

        // 新增使用者
        public async Task<IEnumerable<GetUserListALLDto>> AddUserAsync(PostUserParameter Values)
        {
            // Transaction
            await using var transaction = await _kdbContext.Database.BeginTransactionAsync();
            try
            {
                var preuser = await _kdbContext.Users
                    .Where(u => u.UserName == Values.Account || u.Email == Values.Email)
                    .FirstOrDefaultAsync();
                if (preuser != null)
                {
                    if (StringHelper.StringEqualsIgnoreSpace(preuser.UserName, Values.Account))
                    {
                        throw new InvalidOperationException($"使用者名稱{Values.Account}已存在");
                    }
                    if (StringHelper.StringEqualsIgnoreSpace(preuser.Email, Values.Email))
                    {
                        throw new InvalidOperationException($"使用者名稱{Values.Email}已存在");
                    }
                    throw new InvalidOperationException("已有相同帳號存在,請確認帳號或信箱!");
                }
                var User = _mapper.Map<Users>(Values);
                User.Creater = LoginUserID;
                //新增 Users
                await _kdbContext.Users.AddAsync(User);
                await _kdbContext.SaveChangesAsync();

                // 給預設角色 Roles User
                var UserRoles = new UserRoles { UserId = User.Id, RoleId = 2 };
                await _kdbContext.UserRoles.AddAsync(UserRoles);
                await _kdbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return _mapper.Map<List<GetUserListALLDto>>(new List<Users> { User });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        //修改使用者 給Admin用
        public async Task<IEnumerable<GetUserListALLDto>> UpdUserAsync(UpdUserParamenter Values)
        {
            var preuser = await _kdbContext.Users
                        .Where(u => (u.UserName == Values.Account || u.Email == Values.Email) && u.Id != Values.Id)
                        .FirstOrDefaultAsync();
            if (preuser != null)
            {
                if (StringHelper.StringEqualsIgnoreSpace(preuser.UserName, Values.Account))
                {
                    throw new InvalidOperationException($"使用者名稱{Values.Account}已存在");
                }
                if (StringHelper.StringEqualsIgnoreSpace(preuser.Email, Values.Email))
                {
                    throw new InvalidOperationException($"使用者名稱{Values.Email}已存在");
                }
                throw new InvalidOperationException("已有相同帳號存在,請確認帳號或信箱!");
            }

            var user = await _kdbContext.Users.FirstOrDefaultAsync(u => u.Id == Values.Id);
            if (user is null)
            {
                throw new InvalidOperationException("帳號不存在!");
            }

            _mapper.Map(Values, user);
            await _kdbContext.SaveChangesAsync();

            return new List<GetUserListALLDto> { _mapper.Map<GetUserListALLDto>(user) };
        }
        // 一般使用者改密碼用
        public async Task<IEnumerable<GetUserDto>> UpdUserPassWordAsync(UpdUserParamenter Values)
        {

            var User = await _kdbContext.Users.FirstOrDefaultAsync(u => u.Id == Values.Id);
            if (User is null)
            {
                throw new InvalidOperationException("帳號不存在!");
            }

            User.PasswordHash = Values.Password;
            User.Active = "U";

            await _kdbContext.SaveChangesAsync();
            return _mapper.Map<List<GetUserDto>>(new List<Users> { User });
        }
        // 註銷帳號用(非完全刪除)
        public async Task<bool> UpdUserActiveAsync(DelUserParamenter Values)
        {

            var User = await _kdbContext.Users.FirstOrDefaultAsync(u => u.Id == Values.Id);
            if (User is null)
            {
                throw new InvalidOperationException("帳號不存在!");
            }

            User.Active = "D";
            await _kdbContext.SaveChangesAsync();
            return true;
        }

        // Admin 直接刪除User 資料
        public async Task<bool> DelUserAsync(DelUserParamenter Values)
        {
            // Transaction
            await using var transaction = await _kdbContext.Database.BeginTransactionAsync();
            try
            {
                var User = await _kdbContext.Users.FirstOrDefaultAsync(u => u.Id == Values.Id);
                if (User is null)
                {
                    throw new InvalidOperationException("帳號不存在!");
                }

                // 角色 Roles 
                var userRoles = await _kdbContext.UserRoles
                                        .Where(u => u.UserId == Values.Id)
                                        .ToListAsync();
                _kdbContext.UserRoles.RemoveRange(userRoles);  //先刪子表

                _kdbContext.Users.Remove(User);  //父表後刪
                await _kdbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

    }
}
