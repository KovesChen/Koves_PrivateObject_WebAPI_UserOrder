using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Models;
using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Parameters
{
    public class UpdOrderParamenter
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        [AllowedValues("Created", "Paid", "Shipped", "Delivered", "Cancelled", "Returned")]
        /// 可用值說明：
        /// - Created   : 已建立
        /// - Paid      : 已付款
        /// - Shipped   : 已出貨
        /// - Delivered : 已送達
        /// - Cancelled : 已取消
        /// - Returned  : 已退貨
        public string status { get; set; }
    }
}
