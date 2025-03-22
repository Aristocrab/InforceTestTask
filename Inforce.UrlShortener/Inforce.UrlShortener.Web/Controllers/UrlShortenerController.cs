using FluentResults.Extensions.AspNetCore;
using Inforce.UrlShortener.Application.Interfaces;
using Inforce.UrlShortener.Web.Controllers.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Inforce.UrlShortener.Web.Controllers;

[ApiController]
[Route("api/urls")]
public class UrlShortenerController : BaseController
{
    private readonly IUrlShortenerService _urlShortenerService;

    public UrlShortenerController(IUrlShortenerService urlShortenerService)
    {
        _urlShortenerService = urlShortenerService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetUrls()
    {
        var result = await _urlShortenerService.GetUrls();

        return result.ToActionResult();
    }
    
    [HttpGet("/{shortCode}")]
    public async Task<IActionResult> RedirectToUrl(string shortCode)
    {
        var result = await _urlShortenerService.GetOriginalUrl(shortCode);

        if (result.IsFailed)
        {
            return NotFound();
        }

        return Redirect(result.Value);
    }
    
    [HttpPost("short-code")]
    public async Task<IActionResult> GetShortCode(string originalUrl)
    {
        var userId = GetCurrentUserId();
        
        var result = await _urlShortenerService.GetShortCode(originalUrl, userId);

        return result.ToActionResult();
    }
    
    [HttpGet("original-url")]
    public async Task<IActionResult> GetOriginalUrl(string shortUrl)
    {
        var result = await _urlShortenerService.GetOriginalUrl(shortUrl);

        return result.ToActionResult();
    }
    
    [HttpGet("info")]
    public async Task<IActionResult> GetUrlInfo(Guid urlId)
    {
        var userId = GetCurrentUserId();
        
        var result = await _urlShortenerService.GetUrlInfo(urlId, userId);

        return result.ToActionResult();
    }
    
    [HttpGet("info/{shortCode}")]
    public async Task<IActionResult> GetUrlInfo(string shortCode)
    {
        var userId = GetCurrentUserId();
        
        var result = await _urlShortenerService.GetUrlInfo(shortCode, userId);

        return result.ToActionResult();
    }
    
    [HttpDelete("{urlId}")]
    public async Task<IActionResult> DeleteUrl(Guid urlId)
    {
        var userId = GetCurrentUserId();
        
        var result = await _urlShortenerService.DeleteUrl(urlId, userId);

        return result.ToActionResult();
    }
}