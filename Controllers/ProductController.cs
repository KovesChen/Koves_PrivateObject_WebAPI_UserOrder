using AutoMapper;
using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Models;
using Koves.UserOrder.WebApi.Parameters;
using Koves.UserOrder.WebApi.Services;
using Koves.UserOrder.WebApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
/**
 * Koves 開發專案_商品清單維護 Controller
 */
namespace Koves.UserOrder.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductController(IMapper mapper, IProductService productService)
        {
            _mapper = mapper;
            _productService = productService;
        }
        protected int UserID => _productService.LoginUserID;
        protected string UserRole => _productService.LoginUserRoles;

        /*
         * 查詢商品清單
         * */
        [HttpGet]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> GetProduct([FromQuery] GetProductListParamenter Values)
        {
            try
            {
                if (StringHelper.StringEqualsIgnoreSpace(this.UserRole, "Admin"))
                {
                    var allProductList = await _productService.GetProductListAllInfoAsync(Values);
                    return Ok(ResponseDto<IEnumerable<GetProductListALLInfoDto>>.SuccessResponse(allProductList, "取得成功"));
                }
                else
                {
                    var productList = await _productService.GetProductListAsync(Values);
                    return Ok(ResponseDto<IEnumerable<GetProductListDto>>.SuccessResponse(productList, "取得成功"));
                }
            }
            catch (Exception ex)
            {
                string Message = $"商品清單取得失敗";
                return BadRequest(ResponseDto<object>.FailResponse(this.UserRole,
                                    Message, ex));
            }
        }
        /**
         * 新增商品
         */
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProduct([FromBody] PostProductParamenter Values)
        {
            try
            {
                var result = await _productService.AddProductAsync(Values);
                var Result = _mapper.Map<IEnumerable<GetProductListALLInfoDto>>(result);

                return Ok(ResponseDto<IEnumerable<GetProductListALLInfoDto>>.SuccessResponse(Result, "新增成功"));
            }
            catch (Exception ex)
            {
                string Message = $"商品名 {Values.ProductName} 資料新增失敗";
                return BadRequest(ResponseDto<object>.FailResponse(this.UserRole,
                                    Message, ex));
            }
        }
        /**
         * 修改商品資料
         */
        [HttpPatch]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdProduct([FromBody] updProductParamenter Values)
        {
            try
            {
                var updAll = await _productService.UpdProductAsync(Values);
                return Ok(ResponseDto<IEnumerable<GetProductListALLInfoDto>>.SuccessResponse(updAll, "更新成功"));
            }
            catch (Exception ex)
            {
                string Message = $"商品 {Values.ProductId} 資料更新失敗";
                return BadRequest(ResponseDto<object>.FailResponse(this.UserRole,
                                    Message, ex));
            }
        }
        /**
         * 產品失效 (不是真的刪除只壓生效狀態)
         */
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DelProduct([FromQuery] DelProductParamenter Values)
        {
            try
            {
                bool result = await _productService.DelProductAsync(Values);

                string Message = "產品 =>" + Values.ProductId + (result ? "刪除成功" : "刪除失敗");
                return Ok(ResponseDto<bool>.SuccessResponse(result, Message));
            }
            catch (Exception ex)
            {
                string Message = $"產品 {Values.ProductId} 刪除失敗";
                return BadRequest(ResponseDto<object>.FailResponse(this.UserRole,
                                    Message, ex));
            }
        }

    }
}
