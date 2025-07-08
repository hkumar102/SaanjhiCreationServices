using MediaService.Application.Commands;

namespace MediaService.Application.Handlers;

using AutoMapper;
using MediatR;
using Domain.Entities;
using Infrastructure.Persistence;
using Shared.Contracts.Media;


public class CreateMediaCommandHandler(MediaDbContext db, IMapper mapper)
    : IRequestHandler<CreateMediaCommand, MediaDto>
{
    public async Task<MediaDto> Handle(CreateMediaCommand request, CancellationToken cancellationToken)
    {
        var entity = mapper.Map<Media>(request);
        db.Medias.Add(entity);
        await db.SaveChangesAsync(cancellationToken);
        return mapper.Map<MediaDto>(entity);
    }
}