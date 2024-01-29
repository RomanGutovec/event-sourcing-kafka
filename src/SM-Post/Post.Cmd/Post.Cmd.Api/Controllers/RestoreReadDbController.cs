using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Common.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class RestoreReadDbController : ControllerBase
{
    private readonly ILogger<RestoreReadDbController> _logger;
    private readonly ICommandDispatcher _commandDispatcher;

    public RestoreReadDbController(ILogger<RestoreReadDbController> logger, ICommandDispatcher commandDispatcher)
    {
        _logger = logger;
        _commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult> RestoreReadDbAsync(RestoreReadDbCommand command)
    {
        try
        {
            await _commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new BaseResponse
            {
                Message = "Restore read db request completed successfully."
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
    
        catch (Exception ex)
        {
            var message = $"An error occurred while processing the command restore db: {command.Id}";
            _logger.LogError(ex, message);
            return StatusCode(StatusCodes.Status500InternalServerError, new BaseResponse
            {
                Message = message
            });
        }
    }
}