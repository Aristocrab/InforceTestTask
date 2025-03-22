using Inforce.UrlShortener.Domain.Entities.Shared;

namespace Inforce.UrlShortener.Domain.Entities;

public class ShortUrl : Entity
{
    public string OriginalUrl { get; set; }
    public string ShortCode { get; set; }
    public User CreatedBy { get; set; }

    private ShortUrl() {}
    
    public ShortUrl(string originalUrl, string shortCode, User createdBy)
    {
        OriginalUrl = originalUrl;
        ShortCode = shortCode;
        CreatedBy = createdBy;
    }
}