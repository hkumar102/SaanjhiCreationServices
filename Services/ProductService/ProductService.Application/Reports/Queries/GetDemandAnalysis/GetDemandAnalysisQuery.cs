using MediatR;
using ProductService.Contracts.DTOs;

namespace ProductService.Application.Reports.Queries.GetDemandAnalysis;

public record GetDemandAnalysisQuery(
    List<Guid>? CategoryIds = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    string? AnalysisType = "size" // "size", "color", "category", "all"
) : IRequest<List<DemandAnalysisReportDto>>;
