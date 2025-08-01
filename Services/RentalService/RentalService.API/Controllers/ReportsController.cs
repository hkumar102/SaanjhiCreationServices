using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RentalService.Application.Reports.Rentals;
using RentalService.Contracts.Reports;

namespace RentalService.API.Controllers;

[ApiController]
[Route("api/reports/rentals")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("revenue")]
    public async Task<ActionResult<RentalRevenueReportDto>> GetRevenue([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRentalRevenueReportQuery { FromDate = fromDate, ToDate = toDate }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("profit")]
    public async Task<ActionResult<RentalProfitReportDto>> GetProfit([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRentalProfitReportQuery { FromDate = fromDate, ToDate = toDate }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("activity")]
    public async Task<ActionResult<RentalsActivityReportDto>> GetActivity([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRentalsActivityReportQuery { FromDate = fromDate, ToDate = toDate }, cancellationToken);
        return Ok(result);
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<RentalDashboardReportDto>> GetDashboard([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRentalDashboardReportQuery { FromDate = fromDate, ToDate = toDate }, cancellationToken);
        return Ok(result);
    }
}
