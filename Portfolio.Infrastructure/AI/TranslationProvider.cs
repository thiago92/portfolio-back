using System.Text.Json;
using Portfolio.Domain.Interface;

namespace Portfolio.Infrastructure.AI
{
    public sealed class TranslationProvider : ITranslationProvider
    {
        private readonly Dictionary<string, IReadOnlyDictionary<string, string>> _dictionaries = new();

        public TranslationProvider()
        {
            Load("pt", "Portfolio.Infrastructure.AI.Translations.pt.json");
            Load("en", "Portfolio.Infrastructure.AI.Translations.en.json");
        }

        public IReadOnlyDictionary<string, string>? GetDictionary(string language)
        {
            if (string.IsNullOrWhiteSpace(language)) return null;
            return _dictionaries.TryGetValue(language.ToLowerInvariant(), out var dict) ? dict : null;
        }

        private void Load(string key, string resourceName)
        {
            var assembly = typeof(TranslationProvider).Assembly;
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null) return;

            using var reader = new StreamReader(stream);
            var json = reader.ReadToEnd();
            var dict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            if (dict is not null) _dictionaries[key] = dict;
        }
    }
}
