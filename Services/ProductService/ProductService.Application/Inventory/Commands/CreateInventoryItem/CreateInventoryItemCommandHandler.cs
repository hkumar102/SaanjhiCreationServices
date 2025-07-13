using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence;
using System.Drawing;
using ZXing;
using ZXing.Common;

namespace ProductService.Application.Inventory.Commands.CreateInventoryItem;

public class CreateInventoryItemCommandHandler(
    ProductDbContext db, 
    IMapper mapper,
    ILogger<CreateInventoryItemCommandHandler> logger)
    : IRequestHandler<CreateInventoryItemCommand, InventoryItemDto>
{
    public async Task<InventoryItemDto> Handle(CreateInventoryItemCommand request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting CreateInventoryItemCommand execution for ProductId: {ProductId}", request.ProductId);

        try
        {
            // Verify product exists and get available sizes/colors
            var product = await db.Products.FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");
            }

            // Validate size and color are available for this product
            if (!product.AvailableSizes.Contains(request.Size))
            {
                throw new ArgumentException($"Size '{request.Size}' is not available for this product");
            }

            if (!product.AvailableColors.Contains(request.Color))
            {
                throw new ArgumentException($"Color '{request.Color}' is not available for this product");
            }

            // Generate serial number
            var serialNumber = GenerateSerialNumber(request.ProductId);

            // Generate barcode image as Base64
            var barcodeImageBase64 = GenerateBarcodeBase64(serialNumber);

            // Create inventory item
            var inventoryItem = new InventoryItem
            {
                Id = Guid.NewGuid(),
                ProductId = request.ProductId,
                Size = request.Size,
                Color = request.Color,
                Condition = request.Condition,
                Status = Contracts.Enums.InventoryStatus.Available,
                AcquisitionCost = request.AcquisitionCost,
                AcquisitionDate = request.AcquisitionDate ?? DateTime.UtcNow,
                ConditionNotes = request.ConditionNotes,
                IsRetired = false,
                TimesRented = 0,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                SerialNumber = serialNumber,
                BarcodeImageBase64 = barcodeImageBase64,
                WarehouseLocation = request.WarehouseLocation
            };

            db.InventoryItems.Add(inventoryItem);
            await db.SaveChangesAsync(cancellationToken);

            logger.LogDebug("Successfully created inventory item with ID: {InventoryItemId}, SerialNumber: {SerialNumber}", 
                inventoryItem.Id, inventoryItem.SerialNumber);

            var inventoryItemDto = mapper.Map<InventoryItemDto>(inventoryItem);
            return inventoryItemDto;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing CreateInventoryItemCommand for ProductId: {ProductId}", request.ProductId);
            throw;
        }
    }

    private static string GenerateSerialNumber(Guid productId)
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"SAANJHI-{productId}-{timestamp}-{random}";
    }

    private static string GenerateBarcodeBase64(string value)
    {
        // ZXing.Net barcode generation
        var writer = new BarcodeWriter<Bitmap>
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions { Width = 300, Height = 100 }
        };
        using var bitmap = writer.Write(value);
        using var ms = new System.IO.MemoryStream();
        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return Convert.ToBase64String(ms.ToArray());
    }
}
