using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentResults;
using Inforce.UrlShortener.Application.Database;
using Inforce.UrlShortener.Application.Interfaces;
using Inforce.UrlShortener.Domain.Entities;
using Inforce.UrlShortener.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Inforce.UrlShortener.Application.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext dbContext, 
        IPasswordHashingService passwordHashingService, 
        IConfiguration configuration,
        ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _passwordHashingService = passwordHashingService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<Result<User>> GetUser(Guid userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (user is null)
        {
            return Result.Fail<User>("User does not exist");
        }

        return Result.Ok(user);
    }

    public async Task<Result<string>> Login(string username, string password)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == username);

        if (user is null)
        {
            return Result.Fail("User with this username does not exist");
        }

        if (!_passwordHashingService.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
        {
            return Result.Fail("Invalid password");
        }
        
        _logger.LogInformation("User logged in: {Username}", username);
        
        var jwtToken = GenerateJwtToken(user.Id, user.Role);
        
        return Result.Ok(jwtToken);
    }

    public async Task<Result<string>> Register(string username, string password, Role role = Role.User)
    {
        if (await _dbContext.Users.AnyAsync(x => x.Username == username))
        {
            return Result.Fail("User with this username already exists");
        }
        
        var (hash, salt) = _passwordHashingService.HashPassword(password);

        var user = new User(username, hash, salt, role);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        
        _logger.LogInformation("User registered: {Username}", username);
        
        var jwtToken = GenerateJwtToken(user.Id, user.Role);
        
        return Result.Ok(jwtToken);
    }
    
    private string GenerateJwtToken(Guid userId, Role role)
    {
        var claims = new List<Claim>
        {
            new("userId", userId.ToString()),
            new("role", role.ToString())
        };

        var secretBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"] ?? "");
        var key = new SymmetricSecurityKey(secretBytes);
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            audience: _configuration["Jwt:Audience"],
            issuer: _configuration["Jwt:Issuer"],
            claims: claims,
            signingCredentials: signingCredentials);

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        return jwtToken;
    }
}