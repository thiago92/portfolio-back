namespace Portfolio.Infrastructure.AI
{
    public sealed class AnthropicSettings
    {
        public const string SectionName = "Anthropic";

        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "claude-opus-4-7";
        public int MaxTokens { get; set; } = 2048;
    }
}
