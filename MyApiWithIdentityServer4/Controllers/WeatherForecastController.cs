using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.Uow.Attributes;
using Ars.Common.EFCore.Extension;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using MyApiWithIdentityServer4.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Transactions;

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

            await MyDbContext.Students.AddAsync(new Model.Student
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

            await MyDbContext.SaveChangesAsync();

        }

        [HttpGet(nameof(Query))]
        //[Authorize]
        public async Task<IActionResult> Query()
        {
            var ccc = TestService;
            await ccc.Test();

            var m = await MyDbContext.Students.ToListAsync();
            var n = await MyDbContext.Students.Include(r => r.Enrollments).ToListAsync();
            var o = await MyDbContext.Students.Include(r => r.Enrollments).ThenInclude(r => r.Course).ToListAsync();

            var a = m.First().Enrollments;

            return Ok();
        }

        [Authorize]
        [HttpPost(nameof(Add))]
        public async Task Add()
        {
            await MyDbContext.Students.AddAsync(new Model.Student
            {
                LastName = "bo",
                FirstMidName = "Yang",
                EnrollmentDate = DateTime.UtcNow,
            });

            await MyDbContext.SaveChangesAsync();
        }

        [Authorize]
        [HttpPost(nameof(Modify))]
        public async Task Modify()
        {
            var info = await MyDbContext.Students.FirstOrDefaultAsync();
            info.LastName = "boo";

            await MyDbContext.SaveChangesAsync();
        }

        [Authorize]
        [HttpPost(nameof(Delete))]
        public async Task Delete()
        {
            var info = await MyDbContext.Students.FirstOrDefaultAsync();
            MyDbContext.Students.Remove(info);

            await MyDbContext.SaveChangesAsync();
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

        [HttpPost(nameof(TestJObject))]
        public async Task TestJObject() 
        {
            using var httpclient = _httpClientFactory.CreateClient("http");
            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "name","bill"}
            };
            StringContent stringContent = new StringContent(JsonConvert.SerializeObject(data),Encoding.UTF8,"application/json");
            var res = await httpclient.PostAsync("http://localhost:5196/api/WeatherForecast/TestJObject1", stringContent);
            res.EnsureSuccessStatusCode();
            var result = await res.Content.ReadAsStringAsync();
        }

        [HttpPost(nameof(TestJObject1))]
        public async Task TestJObject1(JObject obj)
        {

        }

        [HttpPost(nameof(TestUow))]
        public async Task TestUow() 
        {
            using var scope = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress);
            using var scope1 = UnitOfWorkManager.Begin(TransactionScopeOption.Required);
            MyDbContext _dbContext = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
            await _dbContext.Students.AddAsync(new Model.Student
            {
                LastName = "test1",
                FirstMidName = "test1",
                EnrollmentDate = DateTime.UtcNow,
            });
            await scope1.CompleteAsync();
            await scope.CompleteAsync();
        }

        [UnitOfWork(IsDisabled = true)]
        [HttpPost(nameof(TestUowWithDispose))]
        public async Task TestUowWithDispose()
        {
            MyDbContext _dbContext = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
            await _dbContext.Students.AddAsync(new Model.Student
            {
                LastName = "test1",
                FirstMidName = "test1",
                EnrollmentDate = DateTime.UtcNow,
            });
            await _dbContext.SaveChangesAsync();
        }
    }
}