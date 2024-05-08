using Newtonsoft.Json;
using Vocabularity.Core;

namespace Vocabularity.Service.Language.Entities;

public class Language : Entity
{
    [JsonProperty("language_name")]
    public string Name { get; set; }

    [JsonProperty("language_image")]
    public string LanguageImage { get; set; }

    public Language() : base(true)
    {
    }
}
