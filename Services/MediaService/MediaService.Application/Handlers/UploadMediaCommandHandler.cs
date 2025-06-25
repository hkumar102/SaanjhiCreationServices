using MediaService.Application.Commands;
using MediaService.Application.DTOs;
using MediaService.Application.Interfaces;
using MediatR;

namespace MediaService.Application.Handlers;

public class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, UploadMediaResult>
{
    private readonly IMediaUploader _mediaUploader;

    public UploadMediaCommandHandler(IMediaUploader mediaUploader)
    {
        _mediaUploader = mediaUploader;
    }

    public async Task<UploadMediaResult> Handle(UploadMediaCommand request, CancellationToken cancellationToken)
    {
        return await _mediaUploader.UploadAsync(request.File, request.MediaType, cancellationToken);
    }
}
