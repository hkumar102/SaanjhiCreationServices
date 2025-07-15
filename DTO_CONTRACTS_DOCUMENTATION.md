# ProductService DTOs and Contracts Documentation

## Overview
This document provides a comprehensive overview of all DTOs and contracts created for the refactored ProductService. These DTOs support the new architecture that separates Product (catalog/design) from InventoryItem (physical items).

## DTO Categories

### 1. Core Entity DTOs

#### ProductDto.cs
- **Purpose**: Represents product catalog/design information
- **Key Features**:
  - Separated from inventory-specific fields
  - Added design fields (brand, designer, SKU)
  - Added specification arrays (sizes, colors)
  - Added business logic flags (isRentable, isPurchasable)
  - Includes related InventoryItem collection
  - Computed properties for inventory counts

#### InventoryItemDto.cs
- **Purpose**: Represents physical items in inventory
- **Key Features**:
  - Physical properties (size, color, serial number)
  - Status and condition tracking
  - Acquisition information
  - Lifecycle tracking (rentals, maintenance)
  - Audit information

### 2. Management DTOs

#### ProductManagementDto.cs
- **DTOs**: CreateProductDto, UpdateProductDto, ProductOperationResultDto
- **Purpose**: Product creation and update operations
- **Key Features**:
  - Validation attributes for data integrity
  - Separate DTOs for create vs update operations
  - Operation result wrapper with success/error handling

#### InventoryItemManagementDto.cs
- **DTOs**: CreateInventoryItemDto, UpdateInventoryItemDto, InventoryItemStatusUpdateDto
- **Purpose**: Inventory item management operations
- **Key Features**:
  - Streamlined creation process
  - Flexible update operations
  - Status update DTO for external services (RentalService)

### 3. Search and Query DTOs

#### ProductSearchDto.cs
- **DTOs**: ProductSearchDto, ProductSearchResultDto, InventoryAvailabilityDto
- **Purpose**: Product search, filtering, and pagination
- **Key Features**:
  - Comprehensive search filters (price, category, specifications)
  - Pagination support
  - Inventory availability queries
  - Sorting options

### 4. Reporting DTOs

#### ReportingDto.cs
- **DTOs**: InventoryAgingReportDto, ProductPerformanceReportDto, InventoryValuationReportDto, InventoryDashboardDto
- **Purpose**: Business intelligence and reporting
- **Key Features**:
  - Inventory aging analysis
  - Product performance metrics
  - Financial valuation reports
  - Dashboard summary statistics

#### InventorySummaryDto.cs
- **Purpose**: Summary statistics for inventory reporting
- **Key Features**:
  - Counts by status and condition
  - Financial summaries
  - Performance metrics

### 5. Cross-Service Communication DTOs

#### RentalSyncDto.cs
- **DTOs**: ProductRentalSyncDto, InventoryItemRentalDto, BulkInventoryStatusUpdateDto, RentalAvailabilityResponseDto
- **Purpose**: Synchronization with RentalService
- **Key Features**:
  - Product rental configuration data
  - Inventory availability for rentals
  - Bulk status updates from RentalService
  - Availability query responses

#### EventDto.cs
- **DTOs**: ProductCreatedEventDto, ProductUpdatedEventDto, InventoryItemCreatedEventDto, InventoryItemStatusChangedEventDto, etc.
- **Purpose**: Event-driven communication
- **Key Features**:
  - Domain events for cross-service notifications
  - Structured event data with metadata
  - Versioning support for event schema evolution

## Enums

### ItemCondition (ProductService.Contracts.Enums)
```csharp
public enum ItemCondition
{
    New = 1,        // Brand new, never rented
    Excellent = 2,  // Minimal wear, like new
    Good = 3,       // Some wear, good condition
    Fair = 4,       // Noticeable wear, functional
    Poor = 5        // Significant wear, needs repair/replacement
}
```

### InventoryStatus (ProductService.Contracts.Enums)
```csharp
public enum InventoryStatus
{
    Available = 1,      // Ready for rental
    Rented = 2,         // Currently rented out
    Maintenance = 3,    // In maintenance/cleaning
    Damaged = 4,        // Needs repair
    Retired = 5         // No longer available for rental
}
```

## Integration Points

### With RentalService
- **API Endpoints**: Product and inventory availability queries
- **Events**: Status changes, product updates
- **Sync DTOs**: Regular synchronization of rental-relevant data

### With CustomerService
- **Events**: Product availability changes
- **API**: Customer-facing product catalogs

### With OrderService
- **Events**: Product updates, inventory changes
- **API**: Product information for orders

## Migration Considerations

### Data Migration
1. **Product Table**: Add new fields (brand, designer, specifications)
2. **InventoryItem Table**: New table with full schema
3. **Data Transfer**: Move inventory data from Product to InventoryItem

### API Migration
1. **Breaking Changes**: ProductDto structure changes
2. **New Endpoints**: Inventory management, reporting
3. **Versioning**: Consider API versioning for gradual migration

### Frontend Updates
1. **Angular Components**: Update to use new DTO structure
2. **Inventory Management**: New UI for inventory operations
3. **Reporting**: New dashboard and reporting components

## Best Practices

### DTO Design
- Separate DTOs for different operations (Create, Update, Query)
- Include validation attributes where appropriate
- Use composition over inheritance
- Include audit fields for traceability

### Event Design
- Include sufficient context in events
- Use structured event types
- Include versioning for schema evolution
- Keep events immutable

### Cross-Service Communication
- Use dedicated sync DTOs for inter-service communication
- Include timestamp and version information
- Design for eventual consistency
- Handle partial failures gracefully

## Next Steps

1. **Repository Updates**: Update repository interfaces and implementations
2. **Service Layer**: Implement business logic using new DTOs
3. **API Controllers**: Create/update endpoints using new DTOs
4. **Mapping Configuration**: Set up AutoMapper profiles
5. **Validation**: Implement FluentValidation rules
6. **Testing**: Create unit and integration tests
7. **Documentation**: API documentation and swagger configs
