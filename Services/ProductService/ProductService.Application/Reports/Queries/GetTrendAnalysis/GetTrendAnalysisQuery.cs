using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Reports.Queries.GetTrendAnalysis;

public record GetTrendAnalysisQuery(
    List<Guid>? CategoryIds = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string? GroupBy = "month" // "day", "week", "month", "quarter"
) : IRequest<List<TrendAnalysisReportDto>>;
