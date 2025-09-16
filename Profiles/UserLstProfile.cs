using AutoMapper;
using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Models;
using Koves.UserOrder.WebApi.Parameters;
using Microsoft.AspNetCore.Identity;
/**
 * 使用者相關 mapper 
*/
namespace Koves.UserOrder.WebApi.Profiles
{
    public class UserLstProfile : Profile
    {
        public UserLstProfile()
        {
            // 給一般查詢用
            CreateMap<Users, GetUserDto>().ForMember(
                dest => dest.Account,
                opt => opt.MapFrom(src => src.UserName)
                ).ReverseMap();
            // 查詢全部資料用
            CreateMap<Users, GetUserListALLDto>().ReverseMap();
            // 查詢資料轉換
            CreateMap<GetUserDto, GetUserListALLDto>().ForMember(
                dest => dest.UserName,
                opt => opt.MapFrom(src => src.Account)
                ).ReverseMap();

            //User Insert用
            CreateMap<PostUserParameter, Users>()
                .ForMember(
                    dest => dest.Id,
                    opt => opt.MapFrom(src => 0)
                )
                .ForMember(
                    dest => dest.UserName,
                    opt => opt.MapFrom(src => src.Account)
                )
                .ForMember(
                    dest => dest.PasswordHash,
                    opt => opt.MapFrom(src => src.Password)
                )
                .ForMember(
                    dest => dest.Active,
                    opt => opt.MapFrom(src => "A")
                )
                .ForMember(
                    dest => dest.Creater,
                    opt => opt.MapFrom(src => 0)
                )
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => DateTime.Now)
                );
            // 更新用
            CreateMap<UpdUserParamenter, Users>()
                .ForMember(
                    dest => dest.UserName,
                    opt =>
                    {
                        opt.Condition(src => src.Account != null);  // 只有不為 null 才更新
                        opt.MapFrom(src => src.Account);
                    }
                )
                .ForMember(
                    dest => dest.PasswordHash,
                    opt =>
                    {
                        opt.Condition(src => src.Password != null);  // 只有不為 null 才更新
                        opt.MapFrom(src => src.Password);
                    }
                )
                .ForMember(
                    dest => dest.Active,
                    opt => opt.MapFrom(src => "U")
                )
                // 其他欄位一律「非 null 才更新」
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));


        }

    }
}
