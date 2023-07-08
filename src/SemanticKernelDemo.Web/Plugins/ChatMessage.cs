using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace SemanticKernelDemo.Web.Plugins;

internal sealed class ChatMessage : ChatMessageBase
{
    public ChatMessage(AuthorRole role, string content) : base(role, content)
    {
    }
}
