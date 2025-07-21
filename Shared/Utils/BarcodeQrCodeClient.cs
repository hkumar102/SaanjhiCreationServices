using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Shared.Utils
{
    public class BarcodeQrCodeClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _qrCodeUrlTemplate;
        private readonly string _barcodeUrlTemplate;

        public BarcodeQrCodeClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _qrCodeUrlTemplate = configuration["Barcode:QrCodeUrlTemplate"];
            _barcodeUrlTemplate = configuration["Barcode:BarcodeUrlTemplate"];
        }

        public async Task<byte[]> GetQrCodeAsync(string text)
        {
            var url = _qrCodeUrlTemplate.Replace("{TEXT}", Uri.EscapeDataString(text));
            return await _httpClient.GetByteArrayAsync(url);
        }

        public async Task<string> GetQrCodeBase64Async(string text)
        {
            var bytes = await GetQrCodeAsync(text);
            return Convert.ToBase64String(bytes);
        }

        public async Task<byte[]> GetBarcodeAsync(string text)
        {
            var url = _barcodeUrlTemplate.Replace("{TEXT}", Uri.EscapeDataString(text));
            return await _httpClient.GetByteArrayAsync(url);
        }

        public async Task<string> GetBarcodeBase64Async(string text)
        {
            var bytes = await GetBarcodeAsync(text);
            return Convert.ToBase64String(bytes);
        }
    }
}
