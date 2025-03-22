using System.Security.Cryptography;
using System.Text;
using FluentResults;
using Inforce.UrlShortener.Application.Database;
using Inforce.UrlShortener.Application.Interfaces;
using Inforce.UrlShortener.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sqids;

namespace Inforce.UrlShortener.Application.Services;

public class UrlShortenerService : IUrlShortenerService
{
    private readonly AppDbContext _dbContext;
    private readonly SqidsEncoder<int> _sqidsEncoder;
    private readonly ILogger<UrlShortenerService> _logger;

    public UrlShortenerService(AppDbContext dbContext, 
        SqidsEncoder<int> sqidsEncoder, 
        ILogger<UrlShortenerService> logger)
    {
        _dbContext = dbContext;
        _sqidsEncoder = sqidsEncoder;
        _logger = logger;
    }

    public async Task<Result<List<ShortUrl>>> GetUrls()
    {
        var urls = await _dbContext.ShortUrls
            .Include(x => x.CreatedBy)
            .ToListAsync();
        
        return Result.Ok(urls);
    }

    public async Task<Result<string>> GetShortCode(string originalUrl, Guid userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            return Result.Fail("User does not exist");
        }
        
        if (_dbContext.ShortUrls.Any(x => x.OriginalUrl == originalUrl))
        {
            return Result.Fail("Short url already exists");
        }
        
        var shortCode = GenerateShortCode(originalUrl);

        var shortUrl = new ShortUrl(originalUrl, shortCode, user);

        await _dbContext.ShortUrls.AddAsync(shortUrl);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Short url created: {@ShortUrl}", shortUrl);

        return Result.Ok(shortCode);
    }

    public async Task<Result<string>> GetOriginalUrl(string shortUrl)
    {
        var shortCode = shortUrl.Split('/').Last();

        var url = await _dbContext.ShortUrls.FirstOrDefaultAsync(x => x.ShortCode == shortCode);

        if (url is null)
        {
            return Result.Fail("Url was not found");
        }
        
        return Result.Ok(url.OriginalUrl);
    }

    public async Task<Result<ShortUrl>> GetUrlInfo(Guid urlId, Guid userId)
    {
        var url = await _dbContext.ShortUrls
            .Include(x => x.CreatedBy)
            .FirstOrDefaultAsync(x => x.Id == urlId);

        if (url is null)
        {
            return Result.Fail("Url was not found");
        }

        if (userId == Guid.Empty)
        {
            return Result.Fail("Log in to view this url");
        }
        
        return Result.Ok(url);
    }

    public async Task<Result<ShortUrl>> GetUrlInfo(string shortCode, Guid userId)
    {
        var url = await _dbContext.ShortUrls
            .Include(x => x.CreatedBy)
            .FirstOrDefaultAsync(x => x.ShortCode == shortCode);

        if (url is null)
        {
            return Result.Fail("Url was not found");
        }

        if (userId == Guid.Empty)
        {
            return Result.Fail("Log in to view this url");
        }
        
        return Result.Ok(url);
    }

    public async Task<Result> DeleteUrl(Guid urlId, Guid userId)
    {
        var url = _dbContext.ShortUrls
            .Include(x => x.CreatedBy)
            .FirstOrDefault(x => x.Id == urlId);
        
        if (url is null)
        {
            return Result.Fail("Url was not found");
        }
        
        if (url.CreatedBy.Id != userId)
        {
            return Result.Fail("You are not the owner of this url");
        }
        
        _dbContext.ShortUrls.Remove(url);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("Short url deleted: {@ShortUrl}", url);
        
        return Result.Ok();
    }

    private string GenerateShortCode(string url)
    {
        var bytes = Encoding.UTF8.GetBytes(url);
        var hash = SHA256.HashData(bytes);
        var number = Math.Abs(BitConverter.ToInt32(hash.AsSpan()[..8]));
        var shortCode = _sqidsEncoder.Encode(number);
        
        return shortCode;
    }
}