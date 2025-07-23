using System;

namespace RentalService.Contracts.DTOs
{
    public class RentalTimelineDto
    {
        public Guid Id { get; set; }
        public Guid RentalId { get; set; }
        public DateTime ChangedAt { get; set; }
        public int Status { get; set; }
        public required string ChangedByUserId { get; set; }
        public string? Notes { get; set; }
    }
}
