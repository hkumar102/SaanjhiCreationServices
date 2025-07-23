using AutoMapper;
using CustomerService.Contracts.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using CustomerService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Common;

namespace CustomerService.Application.Customers.Queries;

public class GetAllCustomersQueryHandler(
    CustomerDbContext context,
    IMapper mapper,
    ILogger<GetAllCustomersQueryHandler> logger)
    : IRequestHandler<GetAllCustomersQuery, PaginatedResult<CustomerDto>>
{
    public async Task<PaginatedResult<CustomerDto>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting GetAllCustomersQuery execution with Page={Page}, PageSize={PageSize}, Search={Search}",
            request.Page, request.PageSize, request.Search);

        try
        {
            var query = context.Customers.Include(c => c.Addresses).AsQueryable();

            // Filtering (OR logic)
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                logger.LogDebug("Filtering by Search term: {Search}", request.Search);
                query = query.Where(c =>
                    EF.Functions.ILike(c.Name, $"%{request.Search}%") ||
                    EF.Functions.ILike(c.Email, $"%{request.Search}%") ||
                    (c.PhoneNumber != null && EF.Functions.ILike(c.PhoneNumber, $"%{request.Search}%")));
            }

            // Sorting
            if (!string.IsNullOrWhiteSpace(request.SortBy))
            {
                logger.LogDebug("Applying sorting: {SortBy}, Descending={SortDesc}", request.SortBy, request.SortDesc);
                query = request.SortBy.ToLower() switch
                {
                    "name" => request.SortDesc ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                    "email" => request.SortDesc ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),
                    "createdat" => request.SortDesc ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                    _ => query.OrderBy(c => c.CreatedAt) // default fallback
                };
            }
            else
            {
                query = query.OrderBy(c => c.CreatedAt);
            }

            // Count total records before pagination
            logger.LogDebug("Counting total records for pagination");
            var totalCount = await query.CountAsync(cancellationToken);
            logger.LogDebug("Found {TotalCount} total records", totalCount);

            // Pagination
            logger.LogDebug("Applying pagination: Skip={Skip}, Take={Take}",
                (request.Page - 1) * request.PageSize, request.PageSize);
            var pagedQuery = query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);

            var customers = await pagedQuery.ToListAsync(cancellationToken);
            logger.LogDebug("Retrieved {CustomerCount} customers for current page", customers.Count);

            var customerDtos = mapper.Map<List<CustomerDto>>(customers);

            var paginatedResult = new PaginatedResult<CustomerDto>
            {
                Items = customerDtos,
                TotalCount = totalCount,
                PageNumber = request.Page,
                PageSize = request.PageSize
            };

            logger.LogDebug("GetAllCustomersQuery completed successfully. Returning {ItemCount} items, Page {Page}, Total={TotalCount}",
                paginatedResult.Items.Count, paginatedResult.PageNumber, paginatedResult.TotalCount);

            return paginatedResult;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while executing GetAllCustomersQuery with Page={Page}, PageSize={PageSize}",
                request.Page, request.PageSize);
            throw;
        }
    }
}
