using AutoMapper;
using RentalService.Application.Rentals.Commands.Create;
using RentalService.Application.Rentals.Commands.Update;
using RentalService.Contracts.DTOs;
using RentalService.Contracts.Enums;
using RentalService.Domain.Entities;

namespace RentalService.Application.Mappings;

public class RentalMappingProfile : Profile
{
    public RentalMappingProfile()
    {
        // Rental <-> RentalDto
        CreateMap<Rental, RentalDto>()
            .ForMember(dest => dest.Product, opt => opt.Ignore()) // populated externally
            .ForMember(dest => dest.Customer, opt => opt.Ignore()) // populated externally
            .ForMember(dest => dest.ShippingAddress, opt => opt.Ignore());

        // CreateRentalCommand -> Rental
        CreateMap<CreateRentalCommand, Rental>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => RentalStatus.Pending));

        // UpdateRentalCommand -> Rental
        CreateMap<UpdateRentalCommand, Rental>();

        // RentalTimeline <-> RentalTimelineDto
        CreateMap<RentalTimeline, RentalTimelineDto>()
            .ForMember(dest => dest.ChangedByUserId, opt => opt.MapFrom(src => src.CreatedBy))
            .ForMember(dest => dest.ChangedAt, opt => opt.MapFrom(src => src.CreatedAt));
    }
}