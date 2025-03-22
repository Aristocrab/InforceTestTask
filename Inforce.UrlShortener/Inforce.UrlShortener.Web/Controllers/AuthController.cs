using FluentResults.Extensions.AspNetCore;
using Inforce.UrlShortener.Application.Interfaces;
using Inforce.UrlShortener.Web.Controllers.Shared;
using Inforce.UrlShortener.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inforce.UrlShortener.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, IConfiguration configuration, ILogger<AuthController> logger)
    {
        _authService = authService;
        _configuration = configuration;
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _authService.GetUser(GetCurrentUserId());
        
        return user.ToActionResult();
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
    {
        var result = await _authService.Login(loginDto.Username, loginDto.Password);

        if (result.IsSuccess)
        {
            _logger.LogInformation("User logged in: {Username}", loginDto.Username);
            
            Response.Cookies.Append("jwt", result.Value, new CookieOptions
            {
                SameSite = SameSiteMode.Strict,
                Secure = true
            });
            
            return Redirect(_configuration["FrontendUrl"]!); // Redirect user to Angular app
        }

        return BadRequest(result.Errors);
    }
}