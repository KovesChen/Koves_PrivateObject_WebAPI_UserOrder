using AutoMapper;
using AutoMapper.QueryableExtensions;
using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Models;
using Koves.UserOrder.WebApi.Parameters;
using Koves.UserOrder.WebApi.Utilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Koves.UserOrder.WebApi.Services
{
    public class ProductService : IProductService
    {
        private readonly KCdbContext _kdbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProductService(KCdbContext kdbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
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
        //一般查詢用
        public async Task<IEnumerable<GetProductListDto>> GetProductListAsync(GetProductListParamenter Values)
        {
            var result = await GetProductListAllInfoAsync(Values);
            return _mapper.Map<IEnumerable<GetProductListDto>>(result);
        }
        // 可以查全部資料用
        public async Task<IEnumerable<GetProductListALLInfoDto>> GetProductListAllInfoAsync(GetProductListParamenter Values)
        {
            var result = from a in _kdbContext.Products
                         select a;

            if (Values.ProductId != null)
            {
                result = result.Where(a => a.ProductId == Values.ProductId);
            }
            if (Values.ProductName != null)
            {
                result = result.Where(a => a.ProductName == Values.ProductName);
            }
            // User 不能查失效的資料
            if (!StringHelper.StringEqualsIgnoreSpace(this.LoginUserRoles, "Admin"))
            {
                result = result.Where(a => a.Active != "D");
            }

            return await result.ProjectTo<GetProductListALLInfoDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        /**
         * 新增商品
         */
        public async Task<IEnumerable<GetProductListALLInfoDto>> AddProductAsync(PostProductParamenter Values)
        {
            // Transaction
            await using var transaction = await _kdbContext.Database.BeginTransactionAsync();
            try
            {
                var preProduct = await _kdbContext.Products
                       .FirstOrDefaultAsync(a =>a.ProductName == Values.ProductName && a.Active != "D");
                if (preProduct != null) {
                    throw new InvalidOperationException($"商品 {Values.ProductName} 已存在!");
                }                
                var Product = _mapper.Map<Products>(Values);
                Product.Creater = LoginUserID;
                Product.Active = "A";
                //新增 Products
                await _kdbContext.Products.AddAsync(Product);
                await _kdbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return _mapper.Map<List<GetProductListALLInfoDto>>(new List<Products> { Product });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<GetProductListALLInfoDto>> UpdProductAsync(updProductParamenter Values)
        {
            var preProduct = await _kdbContext.Products
                              .FirstOrDefaultAsync(a => a.ProductName == Values.ProductName
                              && a.ProductId != Values.ProductId
                              && a.Active != "D");
            if (preProduct != null)
            {
                throw new InvalidOperationException($"重複商品名{Values.ProductName}已存在!");
            }
            var Product = await _kdbContext.Products.FirstOrDefaultAsync(u => u.ProductId == Values.ProductId);
            if (Product is null)
            {
                throw new InvalidOperationException("商品不存在!");
            }

            _mapper.Map(Values, Product);
            await _kdbContext.SaveChangesAsync();
            return _mapper.Map<List<GetProductListALLInfoDto>>(new List<Products> { Product });
        }

        public async Task<bool> DelProductAsync(DelProductParamenter Values)
        {
            var Product = await _kdbContext.Products.FirstOrDefaultAsync(u => u.ProductId == Values.ProductId);
            if (Product is null)
            {
                throw new InvalidOperationException("商品不存在!");
            }

            Product.Active = "D";
            await _kdbContext.SaveChangesAsync();
            return true;
        }

        
    }
}
