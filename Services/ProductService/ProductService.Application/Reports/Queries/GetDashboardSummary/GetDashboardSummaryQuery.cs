using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Reports.Queries.GetDashboardSummary;

public record GetDashboardSummaryQuery(
    DateTime? FromDate = null,
    DateTime? ToDate = null
) : IRequest<InventoryDashboardDto>;
