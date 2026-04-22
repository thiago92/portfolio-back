namespace Portfolio.Domain.Interface
{
    public interface ITranslationProvider
    {
        IReadOnlyDictionary<string, string>? GetDictionary(string language);
    }
}
