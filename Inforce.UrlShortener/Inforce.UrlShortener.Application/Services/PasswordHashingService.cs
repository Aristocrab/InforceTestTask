using System.Security.Cryptography;
using Inforce.UrlShortener.Application.Interfaces;

namespace Inforce.UrlShortener.Application.Services;

public class PasswordHashingService : IPasswordHashingService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;
    
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA512;
    
    public (string hash, string salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);
        
        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public bool VerifyPassword(string password, string hash, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);
        var hashBytes = Convert.FromBase64String(hash);
        
        var newHash = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, Iterations, Algorithm, HashSize);
        
        return newHash.SequenceEqual(hashBytes);
    }
}