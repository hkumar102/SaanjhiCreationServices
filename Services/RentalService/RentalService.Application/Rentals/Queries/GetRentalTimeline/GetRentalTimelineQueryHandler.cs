using MediatR;
using Microsoft.EntityFrameworkCore;
using RentalService.Infrastructure.Persistence;
using RentalService.Contracts.DTOs;

namespace RentalService.Application.Rentals.Queries.GetRentalTimeline
{
    public class GetRentalTimelineQueryHandler : IRequestHandler<GetRentalTimelineQuery, List<RentalTimelineDto>>
    {
        private readonly RentalDbContext _context;
        public GetRentalTimelineQueryHandler(RentalDbContext context)
        {
            _context = context;
        }

        public async Task<List<RentalTimelineDto>> Handle(GetRentalTimelineQuery request, CancellationToken cancellationToken)
        {
            var timeline = await _context.RentalTimelines
                .Where(rt => rt.RentalId == request.RentalId)
                .OrderBy(rt => rt.CreatedAt)
                .Select(rt => new RentalTimelineDto
                {
                    Id = rt.Id,
                    RentalId = rt.RentalId,
                    ChangedAt = rt.CreatedAt,
                    Status = rt.Status,
                    Notes = rt.Notes
                })
                .ToListAsync(cancellationToken);
            return timeline;
        }
    }
}
