using System.ComponentModel.DataAnnotations;

namespace SemanticKernelDemo.Web.Options;

public sealed class SemanticKernelOptions
{
    public string ChatModel { get; init; }

    public string CompletionsModel { get; init; }

    public string EmbeddingsModel { get; init; }

    [Required(AllowEmptyStrings = false)]
    public Uri Endpoint { get; init; }

    [Required(AllowEmptyStrings = false)]
    public string Key { get; init; }
}