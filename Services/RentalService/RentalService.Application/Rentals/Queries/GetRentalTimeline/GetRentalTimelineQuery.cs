using MediatR;
using RentalService.Contracts.DTOs;

namespace RentalService.Application.Rentals.Queries.GetRentalTimeline
{
    public class GetRentalTimelineQuery : IRequest<List<RentalTimelineDto>>
    {
        public Guid RentalId { get; set; }
        public GetRentalTimelineQuery(Guid rentalId)
        {
            RentalId = rentalId;
        }
    }
}
