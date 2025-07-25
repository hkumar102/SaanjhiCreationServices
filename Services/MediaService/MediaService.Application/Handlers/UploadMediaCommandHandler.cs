using MediaService.Application.Commands;
using MediaService.Contracts.DTOs;
using MediaService.Contracts.Interfaces;
using MediatR;

namespace MediaService.Application.Handlers;

public class UploadMediaCommandHandler(IMediaUploader mediaUploader)
    : IRequestHandler<UploadMediaCommand, UploadMediaResult>
{
    public async Task<UploadMediaResult> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
    {
        return await mediaUploader.UploadAsync(request.File, request.MediaType, cancellationToken);
    }
}
