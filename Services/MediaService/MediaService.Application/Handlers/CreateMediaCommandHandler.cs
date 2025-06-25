using MediaService.Application.Commands;

namespace MediaService.Application.Handlers;

using AutoMapper;
using MediatR;
using Domain.Entities;
using Infrastructure.Persistence;
using Shared.Contracts.Media;


public class CreateMediaCommandHandler : IRequestHandler<CreateMediaCommand, MediaDto>
{
    private readonly MediaServiceDbContext _db;
    private readonly IMapper _mapper;

    public CreateMediaCommandHandler(MediaServiceDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<MediaDto> Handle(CreateMediaCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Media>(request);
        _db.Medias.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return _mapper.Map<MediaDto>(entity);
    }
}