using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Inforce.UrlShortener.Web.Controllers.Shared;

public class BaseController : ControllerBase
{
    protected Guid GetCurrentUserId()
    {
        if (HttpContext.User.Identity is not ClaimsIdentity identity)
        {
            return Guid.Empty;
        }

        var userClaims = identity.Claims.ToArray();
        if (userClaims.Length == 0)
        {
            return Guid.Empty;
        }

        return Guid.Parse(userClaims.First(x => x.Type == "userId").Value);
    }
}