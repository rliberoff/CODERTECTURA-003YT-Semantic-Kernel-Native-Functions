using System.ComponentModel.DataAnnotations;

namespace SemanticKernelDemo.Web.Controllers.Models;

public class ChatSummaryRequest
{
    [Required(AllowEmptyStrings = false)]
    public string UserId { get; init; }
}
