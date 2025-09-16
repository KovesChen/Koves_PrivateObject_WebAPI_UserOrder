using Koves.UserOrder.WebApi.Interfaces;
using Koves.UserOrder.WebApi.Models;
using Koves.UserOrder.WebApi.Utilities;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace Koves.UserOrder.WebApi.Parameters
{
    public class updProductParamenter : IValidatableObject
    {
        [Required(ErrorMessage = "ProductId is required")]
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "價格需大於 0")]
        public decimal? Price { get; set; }
        [Range(1, 100, ErrorMessage = "庫存量需大於 0")]
        public int? Stock { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(ProductName) && (!Price.HasValue && !Stock.HasValue))
            {
                yield return new ValidationResult("無修改資料,請確認!", new[] { nameof(ProductId) });
            }
        }
    }
}
