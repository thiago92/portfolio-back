using Anthropic;
using Anthropic.Models.Messages;
using Microsoft.Extensions.Options;
using Portfolio.Domain.Interface;

namespace Portfolio.Infrastructure.AI
{
    public sealed class ClaudePortfolioAssistantClient : IPortfolioAssistantClient
    {
        private readonly AnthropicClient _client;
        private readonly AnthropicSettings _settings;

        public ClaudePortfolioAssistantClient(IOptions<AnthropicSettings> options)
        {
            _settings = options.Value;

            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
                throw new InvalidOperationException("Anthropic ApiKey não configurada. Defina via user-secrets ou env var 'Anthropic__ApiKey'.");

            _client = new AnthropicClient { ApiKey = _settings.ApiKey };
        }

        public async Task<string> AskAsync(
            string systemPrompt,
            IReadOnlyList<(string Role, string Content)> history,
            string userMessage,
            CancellationToken cancellationToken = default)
        {
            var messages = new List<MessageParam>();

            foreach (var (role, content) in history)
            {
                if (string.IsNullOrWhiteSpace(content)) continue;
                messages.Add(new MessageParam
                {
                    Role = role.Equals("assistant", StringComparison.OrdinalIgnoreCase) ? Role.Assistant : Role.User,
                    Content = content,
                });
            }

            messages.Add(new MessageParam { Role = Role.User, Content = userMessage });

            var parameters = new MessageCreateParams
            {
                Model = _settings.Model,
                MaxTokens = _settings.MaxTokens,
                System = new List<TextBlockParam>
                {
                    new()
                    {
                        Text = systemPrompt,
                        CacheControl = new CacheControlEphemeral(),
                    },
                },
                Messages = messages,
            };

            var response = await _client.Messages.Create(parameters);

            var textBlocks = response.Content
                .Select(b => b.Value)
                .OfType<TextBlock>()
                .Select(t => t.Text);

            return string.Join(string.Empty, textBlocks);
        }
    }
}
