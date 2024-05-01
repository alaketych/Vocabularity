using System.Text.Json.Serialization;
using Vocabularity.Core;

namespace Vocabularity.Service.User.Entities;

public class User : Entity
{
    [JsonPropertyName("user_login")]
    public string UserLogin { get; set; }

    [JsonPropertyName("user_pseudonym")]
    public string Pseudonym { get; set; }

    [JsonPropertyName("user_password")]
    public string Password { get; set; }

    [JsonIgnore]
    public string PasswordSalt { get; set; }

    public User() : base(true) 
    { 
    }
}
