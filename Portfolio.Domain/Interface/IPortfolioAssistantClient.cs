namespace Portfolio.Domain.Interface
{
    public interface IPortfolioAssistantClient
    {
        Task<string> AskAsync(
            string systemPrompt,
            IReadOnlyList<(string Role, string Content)> history,
            string userMessage,
            CancellationToken cancellationToken = default);
    }
}
