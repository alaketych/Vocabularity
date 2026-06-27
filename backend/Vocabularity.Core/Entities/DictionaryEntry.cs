namespace Vocabularity.Core.Entities;

public class DictionaryEntry : Entity
{
    public string Name { get; set; } = string.Empty;

    public string UserId { get; set; } = string.Empty;

    public string LanguageId { get; set; } = string.Empty;

    public string OriginalWord { get; set; } = string.Empty;

    public string OriginalTranscriptionedWord { get; set; } = string.Empty;

    public string TranslatedWord { get; set; } = string.Empty;

    public DictionaryEntry() : base(true)
    {
    }
}
