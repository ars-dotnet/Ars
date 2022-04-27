using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyApiWithIdentityServer4.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly MyDbContext myDbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, MyDbContext myDbContext)
        {
            _logger = logger;
            this.myDbContext = myDbContext;
        }

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
        [Authorize]
        public async Task<IActionResult> Query()
        {
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
    }
}