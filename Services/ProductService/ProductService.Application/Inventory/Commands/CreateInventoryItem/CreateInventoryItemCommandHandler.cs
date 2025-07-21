using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductService.Contracts.DTOs;
using ProductService.Domain.Entities;
using ProductService.Infrastructure.Persistence;

using ProductService.Application.Common.Utils;
using Shared.Utils;

namespace ProductService.Application.Inventory.Commands.CreateInventoryItem;

public class CreateInventoryItemCommandHandler : IRequestHandler<CreateInventoryItemCommand, InventoryItemDto>
{
    private readonly ProductDbContext db;
    private readonly IMapper mapper;
    private readonly ILogger<CreateInventoryItemCommandHandler> logger;
    private readonly BarcodeQrCodeClient barcodeQrCodeClient;

    public CreateInventoryItemCommandHandler(
        ProductDbContext db,
        IMapper mapper,
        ILogger<CreateInventoryItemCommandHandler> logger,
        BarcodeQrCodeClient barcodeQrCodeClient)
    {
        this.db = db;
        this.mapper = mapper;
        this.logger = logger;
        this.barcodeQrCodeClient = barcodeQrCodeClient;
    }

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

            string serialNumber = SerialNumberGenerator.Generate();

            // Generate barcode and QR code images as Base64 using shared HTTP client
            var barcodeImageBase64 = await barcodeQrCodeClient.GetBarcodeBase64Async(serialNumber);
            var qrCodeImageBase64 = await barcodeQrCodeClient.GetQrCodeBase64Async(serialNumber);

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
                QRCodeImageBase64 = qrCodeImageBase64,
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
}
