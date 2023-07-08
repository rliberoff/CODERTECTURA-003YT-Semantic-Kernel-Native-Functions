using System.ComponentModel.DataAnnotations;

namespace SemanticKernelDemo.Web.Controllers.Models;

public class ChatRequest
{
    [Required(AllowEmptyStrings = false)]
    public string UserId { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string UserName { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string Message { get; init; }
}
