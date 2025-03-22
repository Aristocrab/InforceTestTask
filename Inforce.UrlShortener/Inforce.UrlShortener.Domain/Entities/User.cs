using Inforce.UrlShortener.Domain.Entities.Shared;
using Inforce.UrlShortener.Domain.Enums;

namespace Inforce.UrlShortener.Domain.Entities;

public class User : Entity
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string PasswordSalt { get; set; }
    public Role Role { get; set; }

    private User() {}

    public User(string username, string passwordHash, string passwordSalt, Role role)
    {
        Username = username;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        Role = role;
    }
}