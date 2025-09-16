namespace Koves.UserOrder.WebApi.Utilities
{
    public static class StringHelper
    {
        public static bool IsNullOrEmptyOrWhiteSpace(string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
        public static bool StringEqualsIgnoreSpace(string a, string b)
        {
            return a?.Trim() == b?.Trim();
        }
    }
}
