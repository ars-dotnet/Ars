using Ars.Common.Localization;
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
        private readonly IStringLocalizer _localizer1;
        private readonly IUserAppService userAppService;
        private readonly UserBase userBase;
        private readonly User user;
        private readonly IArstringLocalizer _arstringLocalizer;
        public HomeController(ILogger<HomeController> logger,
            IStringLocalizerFactory factory,
            IUserAppService userAppService,
            UserBase userBase,
            User user,
            IArstringLocalizer arstringLocalizer)
        {
            var assemblyName = new AssemblyName(GetType().Assembly.FullName!);
            _localizer1 = factory.Create("SharedResource", assemblyName.Name!);

            this.userAppService = userAppService;
            this.userBase = userBase;
            this.user = user;
            _arstringLocalizer = arstringLocalizer;
        }

        [HttpGet()]
        public IActionResult About()
        {
            var m = System.Configuration.ConfigurationManager.AppSettings["Test"];

            string str = " loc 2: " + _localizer1["Name"]
                + " loc 3:" + _arstringLocalizer["Name"];

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
            var m = userAppService.GetList().ToList();
            var n = userBase.GetList().ToList();
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            ViewData["Result"] = _arstringLocalizer["Success!"];
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