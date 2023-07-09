using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace SemanticKernelDemo.Web.Plugins.SimpleChatWithMemoryPlugin;

internal sealed class InMemoryChatHistory
{
    private readonly IDictionary<string, ICollection<ChatMessage>> ChatHistory = new ConcurrentDictionary<string, ICollection<ChatMessage>>();

    public void AddMessage(string userId, ChatMessage message)
    {
        if (!ChatHistory.TryGetValue(userId, out var userChatHistory))
        {
            userChatHistory = new Collection<ChatMessage>();
            ChatHistory.Add(userId, userChatHistory);
        }

        userChatHistory.Add(message);
    }

    public IEnumerable<ChatMessage> GetLastMessages(string userId, int lastMessages)
    {
        return !ChatHistory.TryGetValue(userId, out var userChatHistory)
            ? Enumerable.Empty<ChatMessage>()
            : userChatHistory.Skip(Math.Max(0, userChatHistory.Count - lastMessages));
    }

    public IEnumerable<ChatMessage> GetAllMessages(string userId)
    {
        return !ChatHistory.TryGetValue(userId, out var userChatHistory)
            ? Enumerable.Empty<ChatMessage>()
            : userChatHistory;
    }
}
