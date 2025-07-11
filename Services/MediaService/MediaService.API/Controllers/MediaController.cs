using MediaService.Application.DTOs;

namespace MediaService.API.Controllers;

// MediaService.API/Controllers/MediaController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Commands;
using Shared.Contracts.Media;

[ApiController]
[Route("api/media")]
public class MediaController(IMediator mediator) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<ActionResult<UploadMediaResult>> Upload([FromForm] MediaType mediaType, IFormFile file)
    {
        var result = await mediator.Send(new UploadMediaCommand
        {
            File = file,
            MediaType = mediaType
        });

        return Ok(result);
    }
}
