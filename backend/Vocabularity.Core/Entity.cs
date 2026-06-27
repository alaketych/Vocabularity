namespace Vocabularity.Core;

public class Entity
{
    public string Id { get; set; } = string.Empty;

    public int Ttl { get; set; }

    public Entity(bool generateId = true)
    {
        SetDefaultTimeToLive();

        if (generateId)
        {
            Id = Guid.NewGuid().ToString();
        }
    }

    public virtual void SetDefaultTimeToLive()
    {
        Ttl = -1;
    }
}
