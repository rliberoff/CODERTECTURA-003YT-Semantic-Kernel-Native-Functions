using Microsoft.SemanticKernel.AI.ChatCompletion;

namespace SemanticKernelDemo.Web.Plugins.SimpleChatWithMemoryPlugin;

internal sealed class ChatMessage : ChatMessageBase
{
    public ChatMessage(AuthorRole role, string content) : base(role, content)
    {
    }
}
