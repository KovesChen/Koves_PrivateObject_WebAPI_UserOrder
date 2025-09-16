using AutoMapper;
using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Models;
using Koves.UserOrder.WebApi.Parameters;
/**
 * 物流相關 mapper 
*/
namespace Koves.UserOrder.WebApi.Profiles
{
    public class ProductListProfile : Profile
    {
        public ProductListProfile()
        {
            //查詢物品清單 簡單 <<<>>> 複雜
            CreateMap<GetProductListDto, GetProductListALLInfoDto>().ReverseMap();
            // table Products 轉換
            CreateMap<Products, GetProductListALLInfoDto>().ReverseMap();
            CreateMap<Products, GetProductListDto>().ReverseMap();

            //寫入Products轉換
            CreateMap<PostProductParamenter, Products>()
                .ForMember(
                    dest => dest.ProductId,
                    opt => opt.MapFrom(src => 0)
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
            CreateMap<updProductParamenter, Products>()
                .ForMember(dest => dest.ProductName, opt => opt.Condition(src => src.ProductName != null))
                .ForMember(dest => dest.Price, opt =>
                {
                    opt.PreCondition(src => src.Price.HasValue);  //數值參數, null判斷不了
                    opt.MapFrom(src => src.Price.Value);
                })
                .ForMember(dest => dest.Stock, opt =>
                {
                    opt.PreCondition(src => src.Stock.HasValue);
                    opt.MapFrom(src => src.Stock.Value);
                })
                .ForMember(dest => dest.Active, opt => opt.MapFrom(src => "U"));
        }
    }
}
