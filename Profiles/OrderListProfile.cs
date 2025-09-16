using AutoMapper;
using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Models;

namespace Koves.UserOrder.WebApi.Profiles
{
    public class OrderListProfile : Profile
    {
        public OrderListProfile()
        {
            // 母項table Orders 轉換
            CreateMap<Orders, GetOrderListDto>().ReverseMap();
            CreateMap<Orders, GetOrderListALLDto>().ReverseMap();
            //查詢物品清單 簡單 <<<>>> 複雜
            CreateMap<GetOrderListDto, GetOrderListALLDto>().ReverseMap();

            //子項清單轉換 OrderItem
            CreateMap<OrderItems, GetOrderItemListDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Products.ProductName))
                .ReverseMap()
                .ForMember(dest => dest.Products, opt => opt.Ignore()); // 避免循環

            CreateMap<OrderItems, GetOrderItemListALLDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Products.ProductName))
                .ReverseMap()
                .ForMember(dest => dest.Products, opt => opt.Ignore());
            //查詢物品清單 簡單 <<<>>> 複雜
            CreateMap<GetOrderItemListDto, GetOrderItemListALLDto>().ReverseMap();
        }
    }
}
