using MediatR;

namespace ProductService.Application.Inventory.Commands.GenerateInventoryCodes
{
    public class GenerateInventoryCodesCommand : IRequest<GenerateInventoryCodesResult>
    {
        public Guid InventoryItemId { get; set; }
    }

    public class GenerateInventoryCodesResult
    {
        public string BarcodeImageBase64 { get; set; } = string.Empty;
        public string QRCodeImageBase64 { get; set; } = string.Empty;
    }
}
