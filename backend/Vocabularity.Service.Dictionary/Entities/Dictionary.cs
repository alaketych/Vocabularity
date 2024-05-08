using Vocabularity.Core;
using Newtonsoft.Json;

namespace Vocabularity.Service.Dictionary.Entities;

public class Dictionary : Entity
{
    [JsonProperty("Dictionary_name")]
    public string Name { get; set; }

    [JsonProperty("user_id")]
    public string UserId { get; set; }

    [JsonProperty("language_id")]
    public string LanguageId { get; set; }

    [JsonProperty("original_word")]
    public string OriginalWord { get; set; }

    [JsonProperty("original_transcriptioned_word")]
    public string OriginalTranscriptionedWord { get; set; }

    [JsonProperty("translated_word")]
    public string TranslatedWord { get; set; }

    public Dictionary() : base(true) 
    { 
    }
}
