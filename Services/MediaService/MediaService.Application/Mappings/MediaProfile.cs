namespace MediaService.Application.Mappings;

using AutoMapper;
using Domain.Entities;
using Shared.Contracts.Media;
using Commands;


public class MediaProfile : Profile
{
    public MediaProfile()
    {
        CreateMap<Media, MediaDto>().ReverseMap();
        CreateMap<CreateMediaCommand, Media>();
    }
}