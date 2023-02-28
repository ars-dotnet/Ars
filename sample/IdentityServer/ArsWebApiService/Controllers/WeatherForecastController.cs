using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.Configs;
using Ars.Common.Core.Uow.Attributes;
using Ars.Common.EFCore.Extension;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
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
    [Route("Api/[controller]/[action]")]
    public class WeatherForecastController : MyControllerBase
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly ITestScopeService _testScopeService;
        private readonly IArsIdentityClientConfiguration _clientConfiguration;
        //private readonly MyDbContext myDbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger,
            MyDbContext myDbContext,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            //ITestScopeService testScopeService,
            IArsIdentityClientConfiguration arsIdentityClientConfiguration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            //this.myDbContext = myDbContext;
            _httpContextAccessor = httpContextAccessor;
            //_testScopeService = testScopeService;
            _clientConfiguration = arsIdentityClientConfiguration;
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        [HttpGet(Name = "GetWeatherForecast")]
        //[Authorize]
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
        [Authorize]
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