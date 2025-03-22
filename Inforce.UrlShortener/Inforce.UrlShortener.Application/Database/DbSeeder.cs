using Inforce.UrlShortener.Application.Interfaces;
using Inforce.UrlShortener.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Inforce.UrlShortener.Application.Database;

public class DbSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly IAuthService _authService;
    private readonly IUrlShortenerService _urlShortenerService;
    private readonly ILogger<DbSeeder> _logger;

    public DbSeeder(AppDbContext dbContext, 
        IAuthService authService, 
        IUrlShortenerService urlShortenerService, 
        ILogger<DbSeeder> logger)
    {
        _dbContext = dbContext;
        _authService = authService;
        _urlShortenerService = urlShortenerService;
        _logger = logger;
    }
    
    public async Task Seed()
    {
        await _dbContext.Database.MigrateAsync();
        
        if (!_dbContext.Users.Any())
        {
            await _authService.Register("admin", "admin", Role.Admin);
            await _authService.Register("user", "user");
            
            _logger.LogInformation("Seeded users");
        }
        
        if (!_dbContext.ShortUrls.Any())
        {
            var admin = await _dbContext.Users.FirstAsync(x => x.Username == "admin");
            var user = await _dbContext.Users.FirstAsync(x => x.Username == "user");
            
            await _urlShortenerService.GetShortCode("https://google.com", admin.Id);
            await _urlShortenerService.GetShortCode("https://microsoft.com", admin.Id);
            await _urlShortenerService.GetShortCode("https://apple.com", user.Id);
            
            _logger.LogInformation("Seeded short urls");
        }
    }
}