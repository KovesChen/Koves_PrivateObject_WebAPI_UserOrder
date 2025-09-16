using AutoMapper;
using AutoMapper.QueryableExtensions;
using Koves.UserOrder.WebApi.Dtos;
using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Models;
using Koves.UserOrder.WebApi.Parameters;
using Koves.UserOrder.WebApi.Utilities;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Koves.UserOrder.WebApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly KCdbContext _kdbContext;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderService(KCdbContext kdbContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
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

        public async Task<IEnumerable<GetOrderListDto>> GetOrderListAsync(GetOrderListParamenter Values)
        {

            var order = await GetOrderListAllInfoAsync(Values);
            return _mapper.Map<IEnumerable<GetOrderListDto>>(order);
        }

        public async Task<IEnumerable<GetOrderListALLDto>> GetOrderListAllInfoAsync(GetOrderListParamenter Values)
        {
            var orders = _kdbContext.Orders
                            .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.Products)
                            .AsQueryable();  // Queryable延遲執行因為下面還有補充條件

            if (Values.OrderId != null)
            {
                orders = orders.Where(a => a.OrderId == Values.OrderId);
            }
            if (Values.UserId != null)
            {
                orders = orders.Where(a => a.UserId == Values.UserId);
            }

            return await orders.ProjectTo<GetOrderListALLDto>(_mapper.ConfigurationProvider).ToListAsync();
        }
        public async Task<IEnumerable<GetOrderListALLDto>> AddOrderAsync(PostOrderParamenter Values)
        {
            await using var transaction = await _kdbContext.Database.BeginTransactionAsync();
            try
            {
                var User = await _kdbContext.Users.FirstOrDefaultAsync(a => a.Id == Values.UserId);
                if (User == null || User.Active == "D")
                {
                    throw new InvalidOperationException($"使用者 {Values.UserId} 不存在或已失效!");
                }
                var order = new Orders
                {
                    UserId = Values.UserId,
                    Status = "Created",   //剛新增狀態
                    TotalAmount = 0,   // 先 0，撈子項再累加
                    Creater = LoginUserID,
                    CreatedAt = DateTime.Now,
                    OrderItems = new List<OrderItems>()
                };

                // 子項 OrderItems
                foreach (var item in Values.ProductList)
                {
                    // 取得商品資料
                    var product = await _kdbContext.Products.FirstOrDefaultAsync(p => p.ProductId == item.ProductId && p.Active == "A");
                    if (product == null)
                    {
                        throw new InvalidOperationException($"商品 {item.ProductId} 不存在或已失效!");
                    }
                    else
                    {
                        // 確認庫存資料
                        if (product.Stock < item.Quantity)
                        {
                            throw new InvalidOperationException($"商品 {product.ProductName} 庫存不足,請確認!");
                        }
                    }
                        //細項
                        var orderItem = new OrderItems
                        {
                            ProductId = product.ProductId,
                            Quantity = item.Quantity,
                            UnitPrice = product.Price,
                            TotPrice = product.Price * item.Quantity,
                            CreatedAt = DateTime.Now
                        };

                    order.TotalAmount += orderItem.TotPrice;
                    order.OrderItems.Add(orderItem);

                    //更新庫存
                    product.Stock -= item.Quantity;
                }                 
                await _kdbContext.Orders.AddAsync(order);
                await _kdbContext.SaveChangesAsync();                
                await transaction.CommitAsync();
                return _mapper.Map<List<GetOrderListALLDto>>(new List<Orders> { order });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        /**
         * 更新訂單狀態  訂單狀態 (Created/Paid/Shipped)
         *   /// 可用值說明：
            /// - Created   : 已建立
            /// - Paid      : 已付款
            /// - Shipped   : 已出貨
            /// - Delivered : 已送達
            /// - Cancelled : 已取消
            /// - Returned  : 已退貨
         */
        public async Task<IEnumerable<GetOrderListALLDto>> UpdOrderStatusAsync(UpdOrderParamenter Values)
        {
            await using var transaction = await _kdbContext.Database.BeginTransactionAsync();
            try
            {
                var Order = await _kdbContext.Orders.FirstOrDefaultAsync(u => u.OrderId == Values.OrderId);
                if (Order is null)
                {
                    throw new InvalidOperationException("訂單不存在!");
                }
                if (StringHelper.StringEqualsIgnoreSpace(Order.Status, "Delivered"))
                {
                    throw new InvalidOperationException("訂單已完成不可異動!");
                }

                Order.Status = Values.status;   //更新狀態

                // 退庫存的清單
                string[] returnStatus = new[] { "Cancelled", "Returned" };
                if (returnStatus.Contains(Values.status))
                {
                    // 商品清單
                    var orderitems = _kdbContext.OrderItems.Where(a => a.OrderId == Values.OrderId).ToList();
                    foreach (var orderitem in orderitems)
                    {
                        //撈商品庫存 回庫
                        var product = await _kdbContext.Products.FirstOrDefaultAsync(u => u.ProductId == orderitem.ProductId);
                        if (product != null && orderitem.Quantity > 0)
                        {
                            product.Stock += orderitem.Quantity;
                        }
                    }
                }
                await _kdbContext.SaveChangesAsync();
                await transaction.CommitAsync();
                return _mapper.Map<List<GetOrderListALLDto>>(new List<Orders> { Order });
            }catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> DelOrderAsync(DelOrderParamenter Values)
        {
            throw new NotImplementedException();
        }
    }
}
