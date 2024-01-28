using CQRS.Core.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.Commands;
using Post.Cmd.Api.DTOs;

namespace Post.Cmd.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class NewPostController : ControllerBase
{
    private readonly ILogger<NewPostController> logger;
    private readonly ICommandDispatcher commandDispatcher;

    public NewPostController(ILogger<NewPostController> logger, ICommandDispatcher commandDispatcher)
    {
        this.logger = logger;
        this.commandDispatcher = commandDispatcher;
    }

    [HttpPost]
    public async Task<ActionResult> NewPostAsync(NewPostCommand command)
    {
        command.Id = Guid.NewGuid();

        try
        {
            await commandDispatcher.SendAsync(command);
            return StatusCode(StatusCodes.Status201Created, new NewPostResponse()
            {
                Id = command.Id,
                Message = "New post creation request completed successfully."
            });
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, $"Bad request.");
            return BadRequest(new NewPostResponse()
            {
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            var message = $"An error occurred while processing the command: {command.Id}";
            logger.LogError(ex, message);
            return StatusCode(StatusCodes.Status500InternalServerError, new NewPostResponse()
            {
                Id = command.Id,
                Message = message
            });
        }
    }
}