using FluentResults;
using Inforce.UrlShortener.Domain.Entities;

namespace Inforce.UrlShortener.Application.Interfaces;

public interface IUrlShortenerService
{
    Task<Result<List<ShortUrl>>> GetUrls();
    Task<Result<string>> GetShortCode(string originalUrl, Guid userId);
    Task<Result<string>> GetOriginalUrl(string shortUrl);
    Task<Result<ShortUrl>> GetUrlInfo(Guid urlId, Guid userId);
    Task<Result<ShortUrl>> GetUrlInfo(string shortCode, Guid userId);
    Task<Result> DeleteUrl(Guid urlId, Guid userId);
}