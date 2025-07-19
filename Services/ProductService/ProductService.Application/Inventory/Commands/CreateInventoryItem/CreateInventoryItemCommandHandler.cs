using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence;
using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp.Rendering;

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
            // Verify product exists and get available sizes/colors (include Category)
            var product = await db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
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


            // Get full product and category name

            string productName = product.Name;
            string categoryName = product.Category?.Name;
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                throw new Shared.ErrorHandling.BusinessRuleException($"Product is missing Category, please attach category to product first: {productName}");
            }


            // Generate unique serial number with new format
            string serialNumber;
            int serialTries = 0;
            do
            {
                serialNumber = GenerateSerialNumber(request.ProductId, productName, categoryName, request.Color, request.Size);
                serialTries++;
                // If we ever get stuck, break after 10 tries (should never happen)
                if (serialTries > 10)
                    throw new Exception("Failed to generate unique serial number after 10 attempts");
            }
            while (await db.InventoryItems.AnyAsync(i => i.SerialNumber == serialNumber, cancellationToken));

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

    private static string GenerateSerialNumber(Guid productId, string productName, string categoryName, string color, string size)
    {
        // Format: SAANJHI-[PROD3]-[TYPE3]-[COL4]-[SIZE]-[ID4]
        // PROD3: First 3 chars of product name (upper, no spaces)
        // TYPE3: First 3 chars of category name (upper, no spaces)
        // COL4: First 4 chars of color (upper, no spaces)
        // SIZE: Size string (upper, no spaces)
        // ID4: Random 4-digit number

        var prod3 = (productName ?? "XXX").Replace(" ", string.Empty).ToUpper();
        if (prod3.Length > 3) prod3 = prod3.Substring(0, 3);
        else if (prod3.Length < 3) prod3 = prod3.PadRight(3, 'X');

        var type3 = (categoryName ?? "INV").Replace(" ", string.Empty).ToUpper();
        if (type3.Length > 3) type3 = type3.Substring(0, 3);
        else if (type3.Length < 3) type3 = type3.PadRight(3, 'X');

        var col4 = (color ?? "NONE").Replace(" ", string.Empty).ToUpper();
        if (col4.Length > 4) col4 = col4.Substring(0, 4);
        else if (col4.Length < 4) col4 = col4.PadRight(4, 'X');

        var sizePart = (size ?? "").Replace(" ", string.Empty).ToUpper();
        var id4 = Random.Shared.Next(1000, 9999).ToString();
        return $"SAANJHI-{prod3}-{type3}-{col4}-{sizePart}-{id4}";
    }
    
    
    public static byte[] GenerateBarcode(string text, int width = 300, int height = 100)
    {
        var writer = new BarcodeWriter<SKBitmap>
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Width = width,
                Height = height,
                Margin = 10
            },
            Renderer = new SKBitmapRenderer()
        };

        using var bitmap = writer.Write(text);
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);

        return data.ToArray();
    }
    private static string GenerateBarcodeBase64(string value)
    {
        var barcodeArray = GenerateBarcode(value);
        return Convert.ToBase64String(barcodeArray);
    }
}
