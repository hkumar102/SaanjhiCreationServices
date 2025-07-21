using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Infrastructure.Persistence;
using Shared.ErrorHandling;
using ProductService.Application.Common.Utils;
using Shared.Utils;

namespace ProductService.Application.Inventory.Commands.GenerateInventoryCodes
{
    public class GenerateInventoryCodesCommandHandler : IRequestHandler<GenerateInventoryCodesCommand, GenerateInventoryCodesResult>
    {
        private readonly ProductDbContext _db;
        private readonly BarcodeQrCodeClient _barcodeQrCodeClient;

        public GenerateInventoryCodesCommandHandler(ProductDbContext db, BarcodeQrCodeClient barcodeQrCodeClient)
        {
            _db = db;
            _barcodeQrCodeClient = barcodeQrCodeClient;
        }

        public async Task<GenerateInventoryCodesResult> Handle(GenerateInventoryCodesCommand request, CancellationToken cancellationToken)
        {
            var item = await _db.InventoryItems.FirstOrDefaultAsync(i => i.Id == request.InventoryItemId, cancellationToken);
            if (item == null)
                throw new BusinessRuleException($"Inventory item with ID {request.InventoryItemId} not found");

            // Generate serial number if missing
            if (string.IsNullOrWhiteSpace(item.SerialNumber))
            {
                string serialNumber;
                int tries = 0;
                do
                {
                    serialNumber = SerialNumberGenerator.Generate();
                    tries++;
                    if (tries > 10)
                        throw new BusinessRuleException("Failed to generate unique serial number after 10 attempts");
                }
                while (await _db.InventoryItems.AnyAsync(i => i.SerialNumber == serialNumber, cancellationToken));
                item.SerialNumber = serialNumber;
            }

            var barcodeBase64 = await _barcodeQrCodeClient.GetBarcodeBase64Async(item.SerialNumber);
            var qrCodeBase64 = await _barcodeQrCodeClient.GetQrCodeBase64Async(item.SerialNumber);
            item.BarcodeImageBase64 = barcodeBase64;
            item.QRCodeImageBase64 = qrCodeBase64;
            await _db.SaveChangesAsync(cancellationToken);

            return new GenerateInventoryCodesResult
            {
                BarcodeImageBase64 = barcodeBase64,
                QRCodeImageBase64 = qrCodeBase64
            };
        }
    }
}
