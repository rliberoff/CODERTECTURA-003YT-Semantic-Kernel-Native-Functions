using Microsoft.AspNetCore.Mvc;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;

using SemanticKernelDemo.Web.Controllers.Models;
using SemanticKernelDemo.Web.Plugins.SimpleChatWithMemoryPlugin;

namespace SemanticKernelDemo.Web.Controllers;

[ApiController]
[Produces(@"application/json")]
[Route(@"api/[controller]")]
public class DemoController : ControllerBase
{
    private readonly IKernel kernel;

    public DemoController(IKernel kernel)
    {
        this.kernel = kernel;
    }

    [HttpPost(@"chat")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChatResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChatAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        var contextVariables = new ContextVariables();
        contextVariables.Set(@"userMessage", request.Message);
        contextVariables.Set(@"userName", request.UserName);
        contextVariables.Set(@"userId", request.UserId);

        var context = await kernel.RunAsync(contextVariables, cancellationToken, kernel.Skills.GetFunction(nameof(SimpleChatWithMemoryPlugin), nameof(SimpleChatWithMemoryPlugin.ChatAsync)));

        return context.ErrorOccurred
                ? Problem(context.LastErrorDescription)
                : Ok(new ChatResponse()
                {
                    Response = context.Result,
                });
    }

    [HttpPost(@"chat/composed")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChatResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChatComposedAsync(ChatRequest request, CancellationToken cancellationToken)
    {
        var contextVariables = new ContextVariables();
        contextVariables.Set(@"userMessage", request.Message);
        contextVariables.Set(@"userName", request.UserName);
        contextVariables.Set(@"userId", request.UserId);

        var context = await kernel.RunAsync(contextVariables, cancellationToken, kernel.Skills.GetFunction(nameof(SimpleChatWithMemoryPlugin), nameof(SimpleChatWithMemoryPlugin.ChatComposedAsync)));

        return context.ErrorOccurred
                ? Problem(context.LastErrorDescription)
                : Ok(new ChatResponse()
                {
                    Response = context.Result,
                });
    }

    [HttpPost(@"chat/summary")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ChatResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChatSummaryAsync(ChatSummaryRequest request, CancellationToken cancellationToken)
    {
        var contextVariables = new ContextVariables();
        contextVariables.Set(@"userId", request.UserId);

        var context = await kernel.RunAsync(contextVariables, cancellationToken, kernel.Skills.GetFunction(nameof(SimpleChatWithMemoryPlugin), nameof(SimpleChatWithMemoryPlugin.SummarizeChatHistory)));

        if (context.ErrorOccurred)
        {
            return Problem(context.LastErrorDescription);
        }

        if (string.IsNullOrWhiteSpace(context.Result))
        {
            return NotFound();
        }

        return Ok(new ChatResponse()
        {
            Response = context.Result,
        });
    }
}
