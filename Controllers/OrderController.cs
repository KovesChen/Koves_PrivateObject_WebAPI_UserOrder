using AutoMapper;
using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Parameters;
using Koves.UserOrder.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Koves.UserOrder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IOrderService _orderService;

        public OrderController(IMapper mapper, IOrderService orderService)
        {
            _mapper = mapper;
            _orderService = orderService;
        }
        protected int UserID => _orderService.LoginUserID;
        protected string UserRole => _orderService.LoginUserRoles;
        /*
         * 查詢訂單清單
         * */
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetOrder([FromQuery] GetOrderListParamenter Values)
        {
            try
            {
                if (StringHelper.StringEqualsIgnoreSpace(this.UserRole, "Admin"))
                {
                    var allOrderList = await _orderService.GetOrderListAllInfoAsync(Values);
                    return Ok(ResponseDto<IEnumerable<GetOrderListALLDto>>.SuccessResponse(allOrderList, "取得成功"));
                }
                else
                {
                    Values.UserId = UserID;  //User 僅能查自己的訂單
                    var OrderList = await _orderService.GetOrderListAsync(Values);
                    return Ok(ResponseDto<IEnumerable<GetOrderListDto>>.SuccessResponse(OrderList, "取得成功"));
                }
            }
            catch (Exception ex)
            {
                string Message = $"訂單清單取得失敗";
                return BadRequest(ResponseDto<object>.FailResponse(this.UserRole,
                                    Message, ex));
            }
        }
        /**
         * 新增訂單
         */
        [HttpPost]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> AddOrder([FromBody] PostOrderParamenter Values)
        {
            try
            {
                var result = await _orderService.AddOrderAsync(Values);
                if (StringHelper.StringEqualsIgnoreSpace(this.UserRole, "Admin"))
                {
                    return Ok(ResponseDto< IEnumerable<GetOrderListALLDto>>.SuccessResponse(result, "新增成功"));
                }
                else
                {
                    var OrderList = _mapper.Map<IEnumerable<GetOrderListDto>>(result);
                    return Ok(ResponseDto<IEnumerable<GetOrderListDto>>.SuccessResponse(OrderList, "新增成功"));
                }
            }
            catch (Exception ex)
            {
                string Message = $"訂單資料新增失敗";
                return BadRequest(ResponseDto<object>.FailResponse(this.UserRole,
                                    ex.Message, ex));
            }
        }
        /**
         * 修改訂單狀態
         */
        [HttpPatch]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdOrder([FromBody] UpdOrderParamenter Values)
        {
            try
            {
                var updAll = await _orderService.UpdOrderStatusAsync(Values);
                return Ok(ResponseDto<IEnumerable<GetOrderListALLDto>>.SuccessResponse(updAll, "更新成功"));
            }
            catch (Exception ex)
            {
                string Message = $"訂單 {Values.OrderId} 資料更新失敗";
                return BadRequest(ResponseDto<object>.FailResponse(this.UserRole,
                                    Message, ex));
            }
        }
        /**
         * 訂單失效 (不是真的刪除只壓生效狀態)
         */
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DelOrder([FromQuery] DelOrderParamenter Values)
        {
            try
            {
                var updAll = await _orderService.UpdOrderStatusAsync(new UpdOrderParamenter { OrderId = Values.OrderId , status = "Cancelled" });
                return Ok(ResponseDto<IEnumerable<GetOrderListALLDto>>.SuccessResponse(updAll, "刪除成功"));

            }
            catch (Exception ex)
            {
                string Message = $"訂單 {Values.OrderId} 刪除失敗";
                return BadRequest(ResponseDto<object>.FailResponse(this.UserRole,
                                    Message, ex));
            }
        }
    }
}
