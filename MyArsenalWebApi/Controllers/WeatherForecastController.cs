using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyArsenalWebApi.Controllers
{
    [ApiController]
    [Route("/api/[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// route
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        /// <response code="400">校验失败</response>
        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(400)]
        public string GetFromRoute([FromRoute] int id) 
        {
            return "";
        }

        [Route("{id}")]
        [HttpPost]
        [ApiExplorerSettings(IgnoreApi = true)]//swagger隐藏api
        public string PostFromRoute([FromRoute] int id)
        {
            return "";
        }

        [HttpGet]
        public int GetFromQuery([FromQuery] int id) 
        {
            return id;
        }

        [HttpPost]
        public int PostFromQuery([FromQuery] int id)
        {
            return id;
        }

        [HttpGet]
        public int GetFromForm([FromForm] int id) 
        {
            return id;
        }

        [HttpPost]
        public int PostFromForm([FromForm] int id)
        {
            return id;
        }

        [HttpGet]
        public int GetFromBody([FromBody]Data data)
        {
            return data.id;
        }

        [HttpPost]
        public int PostFromBody([FromBody] int id)
        {
            return id;
        }

        [HttpGet]
        public int GetFromServices([FromServices] IServices services, Data data)
        {
            return services.Get(data.id);
        }

        [HttpPost]
        public int PostFromServices([FromServices] IServices services, Data data)
        {
            M(services);
            services.Action(data.id);
            services.Action(() =>
            {
                return data.id;
            });
            return services.Get(data.id);
        }

        [NonAction]
        private int M(IServices services) 
        {
            services.oncpmplte += (o, e) =>
            {
                var mm = int.Parse(o.ToString());
            };
            return 1;
        }

        [HttpGet]
        public int GetFromHeader([FromHeader] int id) 
        {
            return id;
        }

        [HttpPost]
        public int PostFromHeader([FromHeader] int id)
        {
            return id;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public int Upload([FromForm] Data data) 
        {
            return 1;
        }
    }
}
