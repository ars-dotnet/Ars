using Ars.Commom.Tool.Serializer;
using Ars.Common.Core.IDependency;
using Ars.Common.Redis;
using MessagePack;
using Microsoft.AspNetCore.Mvc;

namespace ArsWebApiService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class RedisController : Controller
    {
        [Autowired]
        public IArsCacheProvider ArsCacheProvider { get; set; }

        [Autowired]
        public IArsHCacheProvider ArsHCacheProvider { get; set; }

        [Autowired]
        public IArsSerializer ArsSerializer { get; set; }

        #region test stringCache

        [HttpGet]
        public async Task<IActionResult> GetWithCache()
        {
            var data = await ArsCacheProvider.GetArsCache("GetWithCache").GetAsync("People", (k) =>
            {
                return Task.FromResult(new People("Bill", 168));
            });

            var data1 = await ArsCacheProvider.GetArsCache("GetWithCache").GetAsync("People1", (k) =>
            {
                return Task.FromResult(new People("Tom", 175));
            });

            var data3 = await ArsCacheProvider.GetArsCache("GetWithCache1").GetAsync("People", (k) =>
            {
                return Task.FromResult(ArsSerializer.SerializeToJson(new People("Jerry", 178)));
            });

            string a = string.Empty;
            string b = string.Empty;
            string c = string.Empty;

            a = ArsSerializer.SerializeToJson(data);
            b = ArsSerializer.SerializeToJson(data1);
            c = data3;

            return Ok(string.Concat(a, "||", b, "||", c));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCache()
        {
            var id = await ArsCacheProvider.GetArsCache("GetWithCache").RemoveAsync("People");

            return Ok(id);
        }

        [HttpPost]
        public async Task<IActionResult> ClearCache()
        {
            await ArsCacheProvider.GetArsCache("GetWithCache").ClearAsync();

            return Ok(true);
        }

        #endregion

        #region test hashCache


        [HttpGet]
        public async Task HSet()
        {
            await ArsHCacheProvider.GetArsCache("HCache").HSetAsync("peopel", "name", "白居易");
            await ArsHCacheProvider.GetArsCache("HCache").HSetAsync("peopel", "top", 180);

            await ArsHCacheProvider.GetArsCache("HCache").HMSetAsync("peopel1", new Dictionary<string, object> { { "name", "李白" }, { "top", 181 } });

            await ArsHCacheProvider.GetArsCache("HCache").HSetAsync("peopel2", "p1", new People("杜甫", 182));
        }

        [HttpGet]
        public async Task<IActionResult> HGet()
        {
            var a = await ArsHCacheProvider.GetArsCache("HCache").HGetAsync<string>("peopel", "name");
            await ArsHCacheProvider.GetArsCache("HCache").HSetAsync("peopel", "name", "白居易1123");

            var b = await ArsHCacheProvider.GetArsCache("HCache").HGetAllAsync<string>("peopel");
            var c = await ArsHCacheProvider.GetArsCache("HCache").HMGetAsync<string>("peopel1", "name", "top");

            var d = await ArsHCacheProvider.GetArsCache("HCache").HGetAsync<People>("peopel2", "p1");

            return Ok();
        }

        [HttpGet]
        public async Task HDel()
        {
            var a = await ArsHCacheProvider.GetArsCache("HCache").HDelAsync("peopel", "name");
            var b = await ArsHCacheProvider.GetArsCache("HCache").HDelAsync("peopel1", "top");
        }

        [HttpGet]
        public async Task HClear()
        {
            await ArsHCacheProvider.GetArsCache("HCache").ClearAsync();
        }

        #endregion

        [MessagePackObject]
        public class People
        {
            public People(string name, double top)
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
