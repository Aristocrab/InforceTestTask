using Microsoft.AspNetCore.Mvc;

namespace Inforce.UrlShortener.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }
}