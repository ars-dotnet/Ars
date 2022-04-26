using Ars.Common.AutoFac;
using Ars.Common.Core.IDependency;
using Autofac;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyIdentityWithGithub.Application;
using MyIdentityWithGithub.Models;
using MyWebApi.Extensions;

namespace MyWebApi.Controllers
{
    public class HomeController : Controller
    {
        [Autowired]
        private IUserAppService userAppService { get; set; }
        private readonly ITestAppService testAppService;
        private readonly IStringLocalizer<HomeController> _stringLocalizer;
        private readonly ILifetimeScope lifetimeScope;
        private readonly Config config;
        private readonly UserBase userBase;
        private readonly IEnumerable<ITestAppService> testAppServices;
        public HomeController(IStringLocalizer<HomeController> stringLocalizer,
            //IUserAppService userAppService,
            ITestAppService testAppService,
            ILifetimeScope lifetimeScope,
            Config config,
            UserBase userBase,
            IEnumerable<ITestAppService> testAppServices)
        {
            _stringLocalizer = stringLocalizer;
            //this.userAppService = userAppService;   
            this.testAppService = testAppService;
            this.lifetimeScope = lifetimeScope;
            this.config = config; 
            this.userBase = userBase;
            this.testAppServices = testAppServices;
        }

        [HttpGet("~/")]
        public IActionResult Index()
        {
            var a = userAppService.UserName;
            var b = testAppService.UserName;

            var h1 = userAppService.GetHashCode();
            var h2 = testAppService.GetHashCode();

            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                var aa = scope.Resolve<ITestAppService>();
                var bb = scope.Resolve<ITestAppService>();

                var i = aa.GetHashCode();
                var ii = bb.GetHashCode();
            }

            using (var scope = lifetimeScope.BeginLifetimeScope())
            {
                var aa = scope.Resolve<ITestAppService>();
                var bb = scope.Resolve<ITestAppService>();

                var i = aa.GetHashCode();
                var ii = bb.GetHashCode();
            }

            foreach (var x in testAppServices) 
            {
                var t = x.UserName;
            }

            var age = config.Age;
            var ageh = config.GetHashCode();

            var xx = userBase.UserName;

            ViewData["MyTitle"] = _stringLocalizer["The localised title of my app!"];
            return View(new HomeViewModel());
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        [HttpGet("~/signin")]
        public async Task<IActionResult> SignIn() => View("SignIn", await HttpContext.GetExternalProvidersAsync());

        [HttpPost("~/signin")]
        public async Task<IActionResult> SignIn([FromForm] string provider)
        {
            // Note: the "provider" parameter corresponds to the external
            // authentication provider choosen by the user agent.
            if (string.IsNullOrWhiteSpace(provider))
            {
                return BadRequest();
            }

            if (!await HttpContext.IsProviderSupportedAsync(provider))
            {
                return BadRequest();
            }

            // Instruct the middleware corresponding to the requested external identity
            // provider to redirect the user agent to its own authorization endpoint.
            // Note: the authenticationScheme parameter must match the value configured in Startup.cs
            return Challenge(new AuthenticationProperties { RedirectUri = "/" }, provider);
        }

        [HttpGet("~/signout")]
        [HttpPost("~/signout")]
        public IActionResult SignOutCurrentUser()
        {
            // Instruct the cookies middleware to delete the local cookie created
            // when the user agent is redirected from the external identity provider
            // after a successful authentication flow (e.g Google or Facebook).
            return SignOut(new AuthenticationProperties { RedirectUri = "/" },
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
