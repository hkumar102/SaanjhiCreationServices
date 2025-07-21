namespace ProductService.Application.Common.Utils
{
    public static class SerialNumberGenerator
    {
        public static string Generate()
        {
            var now = DateTime.UtcNow;
            string timestamp = now.ToString("yyyyMMddHHmmssffffff");
            return $"INV-{timestamp}";
        }
    }
}
