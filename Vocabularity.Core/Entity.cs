using System.Text.Json.Serialization;

namespace Vocabularity.Core;

public class Entity
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("ttl")]
    public int Ttl { get; set; }

    public Entity(bool generateId = true)
    {
        SetDefaultTimeToLive();

        if (generateId)
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }

    public virtual void SetDefaultTimeToLive()
    {
        Ttl = -1;
    }
}
