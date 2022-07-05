using Ars.Common.Core.AspNetCore.OutputDtos;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using MyApiWithIdentityServer4.Dtos;
using Newtonsoft.Json;
using System.Text;

namespace MyApiWithIdentityServer4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : MyControllerBase
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly MyDbContext myDbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            MyDbContext myDbContext,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            //this.myDbContext = myDbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };



        [HttpGet(Name = "GetWeatherForecast")]
        [Authorize]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost(nameof(Action))]
        [Authorize]
        public async Task Action()
        {
            Guid id = Guid.NewGuid();

            await myDbContext.Students.AddAsync(new Model.Student
            {
                Id = id,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "Boo",
                LastName = "Yang",
                Enrollments = new[]
                {
                        new Model.Enrollment
                        {
                            EnrollmentID = 1,
                            CourseID = 1,
                            StudentID = id,
                            Grade = Model.Grade.A,
                            Course = new Model.Course
                            {
                                CourseID = 1,
                                Title = "°¥Ñ½",
                                Credits = 100.11m,
                                Name = "°¥Ñ½"
                            }
                        }
                    }
            });

            await myDbContext.SaveChangesAsync();

        }

        [HttpGet(nameof(Query))]
        //[Authorize]
        public async Task<IActionResult> Query()
        {
            var ccc = testService;
            await ccc.Test();

            var m = await myDbContext.Students.ToListAsync();
            var n = await myDbContext.Students.Include(r => r.Enrollments).ToListAsync();
            var o = await myDbContext.Students.Include(r => r.Enrollments).ThenInclude(r => r.Course).ToListAsync();

            var a = m.First().Enrollments;

            return Ok();
        }

        [Authorize]
        [HttpPost(nameof(Add))]
        public async Task Add()
        {
            await myDbContext.Students.AddAsync(new Model.Student
            {
                LastName = "bo",
                FirstMidName = "Yang",
                EnrollmentDate = DateTime.UtcNow,
            });

            await myDbContext.SaveChangesAsync();
        }

        [Authorize]
        [HttpPost(nameof(Modify))]
        public async Task Modify()
        {
            var info = await myDbContext.Students.FirstOrDefaultAsync();
            info.LastName = "boo";

            await myDbContext.SaveChangesAsync();
        }

        [Authorize]
        [HttpPost(nameof(Delete))]
        public async Task Delete()
        {
            var info = await myDbContext.Students.FirstOrDefaultAsync();
            myDbContext.Students.Remove(info);

            await myDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// µÇÂ¼
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost(nameof(Login))]
        public async Task<ArsOutput<LoginOutput>> Login()
        {
            var a = _httpContextAccessor.HttpContext?.Request.Headers;
            if (a?.TryGetValue("Authorization", out StringValues value) ?? false)
            {
                var m = value.ToString().Split(" ");
                string[]? cc = Encoding.UTF8.GetString(Convert.FromBase64String(m[1]))?.Split(":");
                if (cc?.Any() ?? false) 
                {
                    using var httpclient = _httpClientFactory.CreateClient("http");
                    var tokenresponse = await httpclient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                    {
                        ClientId = cc[0],
                        ClientSecret = cc[1],
                        Scope = "defaultApi-scope",
                        GrantType = "client_credentials",
                        Address = "http://localhost:5105/connect/token"
                    });

                    if (tokenresponse.IsError)
                        return ArsOutput<LoginOutput>.Failed(1, tokenresponse.Error);

                    var datas = JsonConvert.DeserializeObject<LoginOutput>(tokenresponse.Json.ToString());
                    return ArsOutput<LoginOutput>.Success(datas);
                }
            }

            return ArsOutput<LoginOutput>.Failed(1,"²ÎÊý´íÎó");
            
        }

        /// <summary>
        /// Ë¢ÐÂtoken
        /// </summary>
        /// <param name="access_token"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost(nameof(RefreshToken))]
        public async Task<ArsOutput<LoginOutput>> RefreshToken(RefreshTokenInput input)
        {
            using var httpclient = _httpClientFactory.CreateClient("http");
            var tokenresponse = await httpclient.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                ClientId = input.client_id,
                ClientSecret = input.client_secret,
                RefreshToken = input.refresh_token,
                Scope = "defaultApi-scope",
                GrantType = "refresh_token",
                Address = "http://localhost:5105/connect/token"
            });

            if (tokenresponse.IsError)
                return ArsOutput<LoginOutput>.Failed(1, tokenresponse.Error);

            var datas = JsonConvert.DeserializeObject<LoginOutput>(tokenresponse.Json.ToString());
            return ArsOutput<LoginOutput>.Success(datas);
        }
    }
}