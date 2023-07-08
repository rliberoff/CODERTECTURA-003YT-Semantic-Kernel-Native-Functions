using System.ComponentModel;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SkillDefinition;

namespace SemanticKernelDemo.Web.Plugins;

internal sealed class SimpleChatWithMemoryPlugin
{
    private readonly IKernel kernel;
    private readonly IChatCompletion chatService;
    private readonly InMemoryChatHistory inMemoryChatHistory;

    private static readonly ChatRequestSettings ChatRequestSettings = new()
    {
        MaxTokens = 1000,
        Temperature = 0.8,
        TopP = 0.5,
    };

    public SimpleChatWithMemoryPlugin(IKernel kernel, InMemoryChatHistory inMemoryChatHistory)
    {
        this.kernel = kernel;
        this.inMemoryChatHistory = inMemoryChatHistory;

        this.chatService = kernel.GetService<IChatCompletion>();
    }

    [SKFunction, SKName(nameof(ChatAsync)), Description(@"Allows people to chat with an AI based on GPT.")]
    public async Task<string> ChatAsync(
        [Description(@"The unique identifier of the user.")] string userId,
        [Description(@"The name of the user.")] string userName,
        [Description(@"The message from the user.")] string userMessage,
        CancellationToken cancellationToken)
    {
        const int LastMessages = 6;

        var prompt = $@"Today is: {{{{TimeSkill.Today}}}}. Current UTC time is: {{{{TimeSkill.Time}}}}. Use the following time zone offset to calculate the time in different cities: {{{{TimeSkill.TimeZoneOffset}}}}. This is a chat between an artificial intelligence (AI) and {userName}. It also has no ability to access data on the Internet, so it should not claim that it can or say that it will go and look things up. Try to be concise with your answers, though it is not required.";

        var a = "Rewrite the next message to reflect an appropriate system message. Preserve it as is.";

        var completionFunction = kernel.CreateSemanticFunction($"{a}\n{prompt}");

        var resultContext = await completionFunction.InvokeAsync(cancellationToken: cancellationToken);

        if (resultContext.ErrorOccurred)
        {
            throw new InvalidOperationException(resultContext.LastErrorDescription);
        }

        var chat = chatService.CreateNewChat(resultContext.Result);

        chat.Messages.AddRange(inMemoryChatHistory.GetLastMessages(userId, LastMessages));

        chat.AddMessage(AuthorRole.User, userMessage);

        var assistantMessage = await chatService.GenerateMessageAsync(chat, ChatRequestSettings, cancellationToken);

        inMemoryChatHistory.AddMessage(userId, new ChatMessage(AuthorRole.User, userMessage));
        inMemoryChatHistory.AddMessage(userId, new ChatMessage(AuthorRole.Assistant, assistantMessage));

        return assistantMessage;
    }

    [SKFunction, SKName(nameof(SummarizeChatHistory)), Description(@"Summarize a user's chat history.")]
    public async Task<string?> SummarizeChatHistory([Description(@"The unique identifier of the user.")] string userId, CancellationToken cancellationToken)
    {
        var currentUserMempory = inMemoryChatHistory.GetAllMessages(userId);

        if (!currentUserMempory.Any())
        {
            return null;
        }

        var contextVariables = new ContextVariables();
        contextVariables.Set(@"input", string.Join("\n\n", currentUserMempory.Select(message => $@"{message.Role.Label.ToUpperInvariant()} - {message.Content}")));

        var resultContext = await kernel.RunAsync(contextVariables, cancellationToken, kernel.Skills.GetFunction(@"SummarizePlugin", @"Summarize"));

        return resultContext.ErrorOccurred
                ? throw new InvalidOperationException(resultContext.LastErrorDescription)
                : resultContext.Result;
    }
}
