using Newtonsoft.Json;
using Vocabularity.Core;

namespace Vocabularity.Service.User.Entities;

public class User : Entity
{
    [JsonProperty("user_login")]
    public string UserLogin { get; set; }

    [JsonProperty("user_pseudonym")]
    public string Pseudonym { get; set; }

    [JsonProperty("user_password")]
    public string Password { get; set; }

    [JsonIgnore]
    public string PasswordSalt { get; set; }

    public User() : base(true) 
    { 
    }
}
