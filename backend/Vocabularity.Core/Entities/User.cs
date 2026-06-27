namespace Vocabularity.Core.Entities;

public class User : Entity
{
    public string UserLogin { get; set; } = string.Empty;

    public string Pseudonym { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string PasswordSalt { get; set; } = string.Empty;

    public User() : base(true)
    {
    }
}
