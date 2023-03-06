using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.Configs;
using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow.Attributes;
using Ars.Common.EFCore.Extension;
using Ars.Common.EFCore.Repository;
using ArsWebApiService.Model;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using MyApiWithIdentityServer4.Dtos;
using MyApiWithIdentityServer4.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
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

        [Autowired]
        public IRepository<Student, Guid> _repo { get; set; }

        [Autowired]
        public IRepository<ClassRoom> _classRepo { get; set; }

        [Autowired]
        public IRepository<ClassRoom, int> _classRepo1 { get; set; }

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

        #region DbContext Without Transaction
        [HttpPost(nameof(ActionWithOutTransaction))]
        [Authorize]
        public async Task ActionWithOutTransaction()
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
                        EnrollmentID = 3,
                        CourseID = 3,
                        StudentID = id,
                        Grade = Model.Grade.A,
                        Course = new Model.Course
                        {
                            CourseID = 3,
                            Title = "2023.03.01.001",
                            Credits = 100.11m,
                            Name = "2023.03.01.001"
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
        [HttpPost(nameof(ModifyWithOutTransaction))]
        public async Task ModifyWithOutTransaction()
        {
            var info = await MyDbContext.Students.FirstOrDefaultAsync();
            info.LastName = "boo";

            await MyDbContext.SaveChangesAsync();
        }

        [Authorize]
        [HttpPost(nameof(DeleteWithOutTransaction))]
        public async Task DeleteWithOutTransaction()
        {
            var info = await MyDbContext.Students.FirstOrDefaultAsync();
            MyDbContext.Students.Remove(info);

            await MyDbContext.SaveChangesAsync();
        }
        #endregion

        #region DbContext with Transaction

        [HttpPost(nameof(TestUowDefault))]
        public async Task TestUowDefault()
        {
            MyDbContext _dbContext = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
            await _dbContext.Students.AddAsync(new Model.Student
            {
                LastName = "TestUowDefault",
                FirstMidName = "TestUowDefault",
                EnrollmentDate = DateTime.UtcNow,
            });
        }

        [HttpPost(nameof(TestUowRequired))]
        public async Task TestUowRequired()
        {
            using var scope1 = UnitOfWorkManager.Begin(TransactionScopeOption.Required);
            MyDbContext _dbContext = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
            await _dbContext.Students.AddAsync(new Model.Student
            {
                LastName = "TestUowRequired",
                FirstMidName = "TestUowRequired",
                EnrollmentDate = DateTime.UtcNow,
            });
            await scope1.CompleteAsync();
        }

        [HttpPost(nameof(TestUowRequiredNew))]
        public async Task TestUowRequiredNew()
        {
            using var scope1 = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew);
            MyDbContext _dbContext = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
            await _dbContext.Students.AddAsync(new Model.Student
            {
                LastName = "TestUowNewRequired",
                FirstMidName = "TestUowNewRequired",
                EnrollmentDate = DateTime.UtcNow,
            });
            await scope1.CompleteAsync();
        }

        [HttpPost(nameof(TestSuppress))]
        public async Task TestSuppress()
        {
            using (var scope = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                MyDbContext _dbContext = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
                await _dbContext.Students.AddAsync(new Model.Student
                {
                    LastName = "Suppress",
                    FirstMidName = "Suppress",
                    EnrollmentDate = DateTime.UtcNow,
                });
                await scope.CompleteAsync();
            }
        }

        [HttpPost(nameof(TestSuppressInnerRequired))]
        public async Task TestSuppressInnerRequired()
        {
            using (var scope = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                MyDbContext _dbContext = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
                await _dbContext.Students.AddAsync(new Model.Student
                {
                    LastName = "SuppressOut",
                    FirstMidName = "SuppressOut",
                    EnrollmentDate = DateTime.UtcNow,
                });

                using var scope1 = UnitOfWorkManager.Begin(TransactionScopeOption.Required);
                MyDbContext dbContext1 = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
                await dbContext1.Students.AddAsync(new Model.Student
                {
                    LastName = "SuppressInner",
                    FirstMidName = "SuppressInner",
                    EnrollmentDate = DateTime.UtcNow,
                });
                await scope1.CompleteAsync(); //不提交事务
                await scope.CompleteAsync();//提交事务
            }
        }

        [UnitOfWork(IsDisabled = true)]
        [HttpPost(nameof(TestUowWithDispose))]
        public async Task TestUowWithDispose()
        {
            using var scope = UnitOfWorkManager.Begin(TransactionScopeOption.Required);
            MyDbContext _dbContext = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
            await _dbContext.Students.AddAsync(new Model.Student
            {
                LastName = "TestUowWithDispose",
                FirstMidName = "TestUowWithDispose",
                EnrollmentDate = DateTime.UtcNow,
            });
            await _dbContext.SaveChangesAsync();
            await scope.CompleteAsync();
        }

        #endregion

        #region IRepository

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var a = await _repo.GetAll().ToListAsync();
            var b = await _repo.GetAllIncluding(r => r.Enrollments).ToListAsync();
            var c = _repo.GetAllList();
            var d = _repo.GetAllList(r => r.Enrollments.Any(t => t.EnrollmentID == 1));
            var e = _repo.FirstOrDefault(r => r.Id == new Guid("8FB45ADF-3F80-45ED-93CB-10A61CE644E9"));

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> InsertWithIdAsync()
        {
            Guid id = Guid.NewGuid();
            var f = await _repo.InsertAsync(new Student
            {
                Id = id,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "7777",
                LastName = "77778",
                Enrollments = new[]
                {
                    new Model.Enrollment
                    {
                        EnrollmentID = 6,
                        CourseID = 6,
                        StudentID = id,
                        Grade = Model.Grade.A,
                        Course = new Model.Course
                        {
                            CourseID = 6,
                            Title = "2023.03.06.002",
                            Credits = 100.11m,
                            Name = "2023.03.06.002"
                        }
                    }
                }
            });

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> InsertWithOutIdAsync()
        {
            var f = await _repo.InsertAsync(new Student
            {
                Id = Guid.NewGuid(),
                EnrollmentDate = DateTime.Now,
                FirstMidName = "6666",
                LastName = "6666",
            });

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync()
        {
            var e = await _repo.FirstOrDefaultAsync(r => r.Id == new Guid("8FB45ADF-3F80-45ED-93CB-10A61CE644E9"));
            e.LastName = "8888";

            foreach (var en in e.Enrollments)
            {
                en.Grade = Grade.C;
                en.Course.Name = "8888";
            }

            await _repo.UpdateAsync(e);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync()
        {
            var h = await _repo.FirstOrDefaultAsync(r => r.Id == new Guid("CAEF9CEF-EBA3-47DA-AAF9-CF2802413F97"));
            await _repo.DeleteAsync(h);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsyncTest()
        {
            var a = await (await _repo.GetAllAsync()).ToListAsync();
            var b = await (await _repo.GetAllIncludingAsync(r => r.Enrollments)).ToListAsync();
            var c = await _repo.GetAllListAsync();
            var d = await _repo.GetAllListAsync(r => r.Enrollments.Any(t => t.EnrollmentID == 1));
            var e = await _repo.FirstOrDefaultAsync(r => r.Id == new Guid("8FB45ADF-3F80-45ED-93CB-10A61CE644E9"));

            return Ok(a);
        }

        [HttpGet]
        [UnitOfWork(IsDisabled=true)]
        public async Task<IActionResult> GetWithOutTransaction()
        {
            Assert.ThrowsAsync<Exception>(() => _repo.GetAll().ToListAsync());

            return Ok();
        }

        #endregion
    }
}