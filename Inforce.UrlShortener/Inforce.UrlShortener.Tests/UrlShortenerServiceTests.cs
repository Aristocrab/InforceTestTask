using Inforce.UrlShortener.Application.Database;
using Inforce.UrlShortener.Application.Services;
using Inforce.UrlShortener.Domain.Entities;
using Inforce.UrlShortener.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;
using Sqids;

namespace Inforce.UrlShortener.Tests;

public class UrlShortenerServiceTests
{
    private readonly AppDbContext _dbContext;
    private readonly UrlShortenerService _service;
    private const string ShortenerDomain = "http://localhost:5023";

    public UrlShortenerServiceTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);
        var sqidsEncoder = new SqidsEncoder<int>();
        var logger = Substitute.For<ILogger<UrlShortenerService>>();
        _service = new UrlShortenerService(_dbContext, sqidsEncoder, logger);
    }

    [Fact]
    public async Task GetUrls_WhenCalled_ShouldReturnListOfShortUrls()
    {
        // Arrange
        var user = new User("testUser", "passwordHash", "passwordSalt", Role.User);
        var shortUrls = new List<ShortUrl>
        {
            new("https://example.com", "abc123", user)
        };
        await _dbContext.Users.AddAsync(user);
        await _dbContext.ShortUrls.AddRangeAsync(shortUrls);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetUrls();

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(1);
        result.Value.First().ShortCode.ShouldBe("abc123");
    }

    [Fact]
    public async Task GetShortCode_WhenUserNotFound_ShouldReturnFailure()
    {
        // Arrange
        const string originalUrl = "https://example.com";
        var userId = Guid.NewGuid();

        // Act
        var result = await _service.GetShortCode(originalUrl, userId);

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.First().Message.ShouldBe("User does not exist");
    }

    [Fact]
    public async Task GetShortCode_WhenUrlAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var user = new User("testUser", "passwordHash", "passwordSalt", Role.User);
        var shortUrl = new ShortUrl("https://example.com", "abc123", user);
        await _dbContext.Users.AddAsync(user);
        await _dbContext.ShortUrls.AddAsync(shortUrl);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetShortCode("https://example.com", user.Id);

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.First().Message.ShouldBe("Short url already exists");
    }

    [Fact]
    public async Task GetOriginalUrl_WhenShortUrlNotFound_ShouldReturnFailure()
    {
        // Act
        var result = await _service.GetOriginalUrl($"{ShortenerDomain}/xyz");

        // Assert
        result.IsFailed.ShouldBeTrue();
        result.Errors.First().Message.ShouldBe("Url was not found");
    }

    [Fact]
    public async Task GetOriginalUrl_WhenShortUrlExists_ShouldReturnOriginalUrl()
    {
        // Arrange
        var user = new User("testUser", "passwordHash", "passwordSalt", Role.User);
        var shortUrl = new ShortUrl("https://example.com", "abc123", user);
        await _dbContext.Users.AddAsync(user);
        await _dbContext.ShortUrls.AddAsync(shortUrl);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _service.GetOriginalUrl($"{ShortenerDomain}/abc123");

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe("https://example.com");
    }
}
