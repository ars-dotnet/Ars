using ArsMvcApp.Models;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyIdentityWithGithub.Models;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;

namespace ArsMvcApp.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStringLocalizer _localizer;
        private readonly IStringLocalizer _localizer1;

        public HomeController(ILogger<HomeController> logger,
            IStringLocalizerFactory factory)
        {
            var type = typeof(SharedResource);
            var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName!);
            _localizer = factory.Create(type);
            _localizer1 = factory.Create(nameof(SharedResource), assemblyName.Name!);
        }

        [HttpGet()]
        public IActionResult About()
        {
            string str = _localizer["RequiredAttribute_ValidationError"]
                + " loc 2: " + _localizer1["RequiredAttribute_ValidationError"];

            return Ok(str);
        }

        [HttpGet]
        public int Get([Required] int? test)
        {
            return test++ ?? 0;
        }

        [HttpGet("~/")]
        public IActionResult Index()
        {
            return View(new HomeViewModel());
        }

        [HttpPost]
        public IActionResult Index([FromForm]HomeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ViewData["Result"] = _localizer["Success!"];
            return View(model);
        }

        [HttpPost]
        public IActionResult SetLanguage([FromForm]string culture, [FromQuery]string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}