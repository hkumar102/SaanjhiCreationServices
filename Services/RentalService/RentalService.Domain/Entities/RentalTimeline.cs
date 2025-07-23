using System;
using Shared.Domain.Entities;

namespace RentalService.Domain.Entities
{
    public class RentalTimeline : BaseEntity
    {
        public Guid RentalId { get; set; }
        public int Status { get; set; } // Use same enum/int as Rental.Status
        public string? Notes { get; set; }

        public Rental Rental { get; set; }
    }
}
