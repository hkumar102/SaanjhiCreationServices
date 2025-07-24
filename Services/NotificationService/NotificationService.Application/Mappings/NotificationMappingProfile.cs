using AutoMapper;
using NotificationService.Domain;
using NotificationService.Contracts.DTOs;

namespace NotificationService.Application.Mappings;

public class NotificationMappingProfile : Profile
{
    public NotificationMappingProfile()
    {
        CreateMap<Notification, NotificationDto>().ReverseMap();
        CreateMap<NotificationTemplate, NotificationTemplateDto>().ReverseMap();
    }
}
