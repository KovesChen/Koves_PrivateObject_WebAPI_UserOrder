using Koves.UserOrder.WebApi.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Koves.UserOrder.WebApi.Dtos
{
    /**
     * API統一回傳格式 DTO 用
     */
    public class ResponseDto<T>
    {
        public bool Success { get; set; }

        public T Result { get; set; } = default!;

        public string Message { get; set; } = "";

        public string Details { get; set; } = "";

        public string Erro { get; set; } = "";

        // 成功回應
        public static ResponseDto<T> SuccessResponse(T result, string message = "成功")
        {
            return new ResponseDto<T>
            {
                Success = true,
                Result = result,
                Message = message
            };
        }

        // 失敗回應
        public static ResponseDto<T> FailResponse(string UserRole, string message, Exception ex)
        {
            string details = "";
            string errorMessage = "";
            // 管理員在顯示程式內部錯誤
            if (StringHelper.StringEqualsIgnoreSpace(UserRole, "Admin"))
            {
                details = ex.Message;
                errorMessage = ex.InnerException?.Message;
            }
            return new ResponseDto<T>
            {
                Success = false,
                Message = message,
                Details = details,
                Erro = errorMessage
            };
        }
    }
}
