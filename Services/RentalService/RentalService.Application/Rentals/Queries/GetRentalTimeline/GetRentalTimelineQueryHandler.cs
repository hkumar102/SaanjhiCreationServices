using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RentalService.Infrastructure.Persistence;
using RentalService.Contracts.DTOs;

namespace RentalService.Application.Rentals.Queries.GetRentalTimeline
{
    public class GetRentalTimelineQueryHandler : IRequestHandler<GetRentalTimelineQuery, List<RentalTimelineDto>>
    {
        private readonly RentalDbContext _context;
        private readonly IMapper _mapper;
        public GetRentalTimelineQueryHandler(RentalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RentalTimelineDto>> Handle(GetRentalTimelineQuery request, CancellationToken cancellationToken)
        {
            var timeline = await _context.RentalTimelines
                .Where(rt => rt.RentalId == request.RentalId)
                .OrderBy(rt => rt.CreatedAt)
                .ProjectTo<RentalTimelineDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);
            return timeline;
        }
    }
}
