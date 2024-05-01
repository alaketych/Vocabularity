using Vocabularity.Core;
using System.Text.Json.Serialization;

namespace Vocabularity.Service.Dictionary.Entities;

public class Dictionary : Entity
{
    [JsonPropertyName("Dictionary_name")]
    public string Name { get; set; }

    [JsonPropertyName("user_id")]
    public string UserId { get; set; }

    [JsonPropertyName("language_id")]
    public string LanguageId { get; set; }

    [JsonPropertyName("original_word")]
    public string OriginalWord { get; set; }

    [JsonPropertyName("original_transcriptioned_word")]
    public string OriginalTranscriptionedWord { get; set; }

    [JsonPropertyName("translated_word")]
    public string TranslatedWord { get; set; }

    public Dictionary() : base(true) 
    { 
    }
}
