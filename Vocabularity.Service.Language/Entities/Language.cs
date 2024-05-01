using System.Text.Json.Serialization;
using Vocabularity.Core;

namespace Vocabularity.Service.Language.Entities;

public class Language : Entity
{
    [JsonPropertyName("language_name")]
    public string Name { get; set; }

    [JsonPropertyName("language_image")]
    public string LanguageImage { get; set; }

    public Language() : base(true)
    {
    }
}
