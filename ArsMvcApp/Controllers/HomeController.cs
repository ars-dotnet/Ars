using Ars.Commom.Tool.Serializer;
using Ars.Common.Localization;
using Ars.Common.Redis;
using ArsMvcApp.Models;
using MessagePack;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using MyIdentityWithGithub.Models;
using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using KeyAttribute = MessagePack.KeyAttribute;

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
        private readonly IArsCacheProvider _arsCacheProvider;
        private readonly IArsSerializer _arsSerializer;
        public HomeController(ILogger<HomeController> logger,
            IStringLocalizerFactory factory,
            IUserAppService userAppService,
            UserBase userBase,
            User user,
            IArstringLocalizer arstringLocalizer,
            IArsCacheProvider arsCacheProvider,
            IArsSerializer arsSerializer)
        {
            var assemblyName = new AssemblyName(GetType().Assembly.FullName!);
            _localizer1 = factory.Create("SharedResource", assemblyName.Name!);

            this.userAppService = userAppService;
            this.userBase = userBase;
            this.user = user;
            _arstringLocalizer = arstringLocalizer;
            _arsCacheProvider = arsCacheProvider;
            _arsSerializer = arsSerializer;
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

        [HttpGet()]
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

        #region test TypeCache
        [HttpGet]
        public async Task<IActionResult> GetCacheAsType()
        {
            var data = await _arsCacheProvider.AsType<string,People>("GetCacheAsType").GetAsync("People", async (k) =>
            {
                return new People("Bill", 168);
            });

            var data1 = await _arsCacheProvider.AsType<string, People>("GetCacheAsType").GetAsync("People1", async (k) =>
            {
                return new People("Tom", 175);
            });

            var data3 = await _arsCacheProvider.AsType<string, People>("GetCacheAsType1").GetAsync("People", async (k) =>
            {
                return new People("Jerry", 178);
            });

            string a = string.Empty;
            string b = string.Empty;
            string c = string.Empty;
            if (data.GetType() == typeof(People))
            {
                a = _arsSerializer.SerializeToJson(data);
                b = _arsSerializer.SerializeToJson(data1);
                c = _arsSerializer.SerializeToJson(data3);
            }

            return Ok(string.Concat(a, "||", b,"||", c));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCacheAsType()
        {
            var id = await _arsCacheProvider.AsType<string, People>("GetCacheAsType").RemoveAsync("People");

            return Ok(id);
        }

        [HttpPost]
        public async Task<IActionResult> ClearCacheAsType()
        {
            await _arsCacheProvider.AsType<string, People>("GetCacheAsType").ClearAsync();

            return Ok(true);
        }

        #endregion

        #region test ICache

        [HttpGet]
        public async Task<IActionResult> GetWithCache() 
        {
            var data = await _arsCacheProvider.GetArsCache("GetWithCache").GetAsync("People" ,async (k) => 
            {
                return new People("Bill",168);
            });

            var data1 = await _arsCacheProvider.GetArsCache("GetWithCache").GetAsync("People1", async (k) =>
            {
                return new People("Tom", 175);
            });

            var data3 = await _arsCacheProvider.GetArsCache("GetWithCache1").GetAsync("People", async (k) =>
            {
                return new People("Jerry", 178);
            });

            string a = string.Empty;
            string b = string.Empty;
            string c = string.Empty;
            if (data.GetType() == typeof(JObject)) 
            {
                a = _arsSerializer.SerializeToJson(((JObject)data).ToObject<People>());
                b = _arsSerializer.SerializeToJson(((JObject)data1).ToObject<People>());
                c = _arsSerializer.SerializeToJson(((JObject)data3).ToObject<People>());
            }
            else if (data.GetType() == typeof(People)) 
            {
                a = _arsSerializer.SerializeToJson((People)data);
                b = _arsSerializer.SerializeToJson((People)data1);
                c = _arsSerializer.SerializeToJson((People)data3);
            }

            return Ok(string.Concat(a,"||",b));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCache() 
        {
            var id = await _arsCacheProvider.GetArsCache("GetWithCache").RemoveAsync("People");

            return Ok(id);
        }

        [HttpPost]
        public async Task<IActionResult> ClearCache()
        {
            await _arsCacheProvider.GetArsCache("GetWithCache").ClearAsync();

            return Ok(true);
        }

        #endregion

        [MessagePackObject]
        public class People 
        {
            public People(string name,double top)
            {
                Name = name;
                Top = top;
            }

            [Key(nameof(Name))]
            public string Name { get; set; }

            [Key(nameof(Top))]
            public double Top { get; set; }
        }
    }
}