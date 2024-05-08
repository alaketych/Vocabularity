using Newtonsoft.Json;

namespace Vocabularity.Core;

public class Entity
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("ttl")]
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
