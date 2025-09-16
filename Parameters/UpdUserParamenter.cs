using Koves.UserOrder.WebApi.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace Koves.UserOrder.WebApi.Parameters
{
    public class UpdUserParamenter : IValidatableObject
    {
        [Required(ErrorMessage = "Id is required")]
        public int Id { get; set; }
       
        [StringLength(15, MinimumLength = 6)]
        public string? Account { get; set; }

        
        [StringLength(15, MinimumLength = 9)]
        public string? Password { get; set; }

        
        [EmailAddress]
        public string? Email { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Account) && string.IsNullOrEmpty(Password) && string.IsNullOrEmpty(Email))
            {
                yield return new ValidationResult("無修改資料,請確認!", new[] { nameof(Email) });
            }
            
        }
    }
}
