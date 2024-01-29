using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class LikePostController : ControllerBase
{
    private readonly ILogger<NewPostController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public LikePostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> LikePostAsync(Guid id, LikePostCommand command)
    {
        try
        {
            await _commandDispatcher.SendAsync(new LikePostCommand() { Id = id });
            return StatusCode(StatusCodes.Status201Created, new BaseResponse
            {
                Message = "Like post request completed successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, $"Bad request.");
            return BadRequest(new BaseResponse
            {
                Message = ex.Message
            });
        }
        catch (AggregateNotFoundException ex)
        {
            _logger.LogWarning(ex, $"Aggregate was not found. Post id: {command.Id}");
            return BadRequest(new BaseResponse
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            var message = $"An error occurred while processing the command: {command.Id}";
            _logger.LogError(ex, message);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                Message = message
            });
        }
    }
}