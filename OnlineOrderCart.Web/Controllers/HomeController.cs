using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineOrderCart.Common.DesignPatternsTools;
using OnlineOrderCart.Web.Configurations;
using OnlineOrderCart.Web.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineOrderCart.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<MyConfiguration> _config;

        public HomeController(IOptions<MyConfiguration> config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            Log.GetInstance(_config.Value.PathLog).Save($"Entro a Index Home   Date :{DateTime.Now.ToUniversalTime()}");
            return View();
        }

        public IActionResult Privacy()
        {
            Log.GetInstance(_config.Value.PathLog).Save($"Entro a Privacy Home Date :{DateTime.Now.ToUniversalTime()}");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult StatusCode404()
        {
            return View();
        }
        public IActionResult StatusCode500()
        {
            return View();
        }

        public IActionResult StatusCode502()
        {
            return View();
        }

        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CultureManagement(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });
            return LocalRedirect(returnUrl);
            //return RedirectToAction(nameof(Index));
        }
    }
}
