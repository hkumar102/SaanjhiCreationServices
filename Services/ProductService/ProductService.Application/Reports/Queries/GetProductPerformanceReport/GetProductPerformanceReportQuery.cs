using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Reports.Queries.GetProductPerformanceReport;

public record GetProductPerformanceReportQuery(
    List<Guid>? CategoryIds = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int TopProducts = 10
) : IRequest<List<ProductPerformanceReportDto>>;
