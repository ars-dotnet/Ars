using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.Configs;
using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Ars.Common.Core.Uow.Attributes;
using Ars.Common.EFCore.AdoNet;
using Ars.Common.EFCore.Extension;
using Ars.Common.EFCore.Repository;
using ArsWebApiService.Model;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using MyApiWithIdentityServer4.Dtos;
using MyApiWithIdentityServer4.Model;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Transactions;

namespace MyApiWithIdentityServer4.Controllers
{
    [ApiController]
    [Route("Api/[controller]/[action]")]
    public class DbContextController : MyControllerBase
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DbContextController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly ITestScopeService _testScopeService;
        private readonly IArsIdentityClientConfiguration _clientConfiguration;
        //private readonly MyDbContext myDbContext;
        private readonly IUnitOfWork _unitOfWork;

        public DbContextController(ILogger<DbContextController> logger,
            MyDbContext myDbContext,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            //ITestScopeService testScopeService,
            IArsIdentityClientConfiguration arsIdentityClientConfiguration,
            IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            //this.myDbContext = myDbContext;
            _httpContextAccessor = httpContextAccessor;
            //_testScopeService = testScopeService;
            _clientConfiguration = arsIdentityClientConfiguration;
            _unitOfWork = unitOfWork;
        }

        [Autowired]
        public IRepository<Student, Guid> _repo { get; set; }

        [Autowired]
        public IRepository<ClassRoom,Guid> _classRepo { get; set; }

        [Autowired]
        public IDbExecuter<MyDbContext> DbExecuter { get; set; }

        [Autowired]
        protected IRepository<AppVersion> RepoApp { get; set; }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };


        [HttpGet(Name = "GetWeatherForecast")]
        //[Authorize]
        public IEnumerable<WeatherForecast> Get([FromServices]IServiceProvider serviceProvider)
        {
            var a = serviceProvider.GetRequiredService<MyDbContext>();
            var b = serviceProvider.GetRequiredService<MyDbContext>();

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
        public async Task<IActionResult> Query()
        {
            var m = await MyDbContext.Students.FirstOrDefaultAsync(r => r.LastName.Equals("8899"));
            var n = await MyDbContext.Students.Include(r => r.Enrollments).FirstOrDefaultAsync(r => r.LastName.Equals("Yang"));
            var o = await MyDbContext.Students.Include(r => r.Enrollments).ThenInclude(r => r.Course).FirstOrDefaultAsync(r => r.LastName.Equals("Yang"));

            return Ok((m,n,o));
        }

        [Authorize]
        [HttpPost(nameof(ModifyWithOutTransaction))]
        public async Task ModifyWithOutTransaction()
        {
            var info = await MyDbContext.Students.FirstOrDefaultAsync();
            info.LastName = "boo" + new Random().Next(20);

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
                LastName = "TestUowDefault11",
                FirstMidName = "TestUowDefault11",
                EnrollmentDate = DateTime.Now,
            });

            string sql = @"insert into Students(Id,LastName,FirstMidName,EnrollmentDate,TenantId,CreationUserId,IsDeleted) " +
                "values(@Id,@LastName,@FirstMidName,@EnrollmentDate,@TenantId,@CreationUserId,@IsDeleted)";
            MySqlParameter[] sqlParameters =
            {
                new MySqlParameter("@Id",Guid.NewGuid()),
                new MySqlParameter("@LastName",8899),
                new MySqlParameter("@FirstMidName","aabb121211"),
                new MySqlParameter("@EnrollmentDate",DateTime.Now),
                new MySqlParameter("@TenantId",1),
                new MySqlParameter("@CreationUserId",1),
                new MySqlParameter("@IsDeleted",false),
            };

            DbExecuter.BeginWithEFCoreTransaction(UnitOfWorkManager.Current!);
            var count = await DbExecuter.ExecuteNonQuery(sql, sqlParameters);

            string updatesql = $"update Students set LastName = @LastName where FirstMidName = @FirstMidName";
            MySqlParameter[] upsqlParameters =
            {
                 new MySqlParameter("@LastName",889999),
                 new MySqlParameter("@FirstMidName","aabb121211"),
            };
            count = await DbExecuter.ExecuteNonQuery(updatesql, upsqlParameters);
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
                EnrollmentDate = DateTime.Now,
            });

            string sql = @"insert into Students(Id,LastName,FirstMidName,EnrollmentDate,TenantId,CreationUserId,IsDeleted) " +
                "values(@Id,@LastName,@FirstMidName,@EnrollmentDate,@TenantId,@CreationUserId,@IsDeleted)";
            MySqlParameter[] sqlParameters =
            {
                new MySqlParameter("@Id",Guid.NewGuid()),
                new MySqlParameter("@LastName",8899),
                new MySqlParameter("@FirstMidName","aabb121212"),
                new MySqlParameter("@EnrollmentDate",DateTime.Now),
                new MySqlParameter("@TenantId",1),
                new MySqlParameter("@CreationUserId",1),
                new MySqlParameter("@IsDeleted",false),
            };

            DbExecuter.BeginWithEFCoreTransaction(UnitOfWorkManager.Current!);
            var count = await DbExecuter.ExecuteNonQuery(sql, sqlParameters);

            string updatesql = $"update Students set LastName = @LastName where FirstMidName = @FirstMidName";
            MySqlParameter[] upsqlParameters =
            {
                 new MySqlParameter("@LastName",889999),
                 new MySqlParameter("@FirstMidName","aabb121212"),
            };
            count = await DbExecuter.ExecuteNonQuery(updatesql, upsqlParameters);

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
            using var scope0 = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew);
            UnitOfWorkManager.Current.Completed += (sender, args) =>
            {

            };
            MyDbContext dbContext0 = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
            await dbContext0.Students.AddAsync(new Model.Student
            {
                LastName = "RequiresNew",
                FirstMidName = "RequiresNew",
                EnrollmentDate = DateTime.UtcNow,
            });
            await scope0.CompleteAsync(); //提交事务

            using (var scope = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                UnitOfWorkManager.Current.Completed += (sender, args) =>
                {

                };
                MyDbContext _dbContext = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
                await _dbContext.Students.AddAsync(new Model.Student
                {
                    LastName = "Suppress.Out.001",
                    FirstMidName = "Suppress.Out.001",
                    EnrollmentDate = DateTime.UtcNow,
                });

                using var scope1 = UnitOfWorkManager.Begin(TransactionScopeOption.Required);
                UnitOfWorkManager.Current.Completed += (sender, args) =>
                {

                };
                MyDbContext dbContext1 = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
                await dbContext1.Students.AddAsync(new Model.Student
                {
                    LastName = "Suppress.Required.Inner.001",
                    FirstMidName = "Suppress.Required.Inner",
                    EnrollmentDate = DateTime.UtcNow,
                });
                await scope1.CompleteAsync(); //直接SaveChangesAsync，没有提交事务
                await scope.CompleteAsync();//提交事务
            }

            using (var scope = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                UnitOfWorkManager.Current.Completed += (sender, args) =>
                {

                };
                MyDbContext _dbContext = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
                await _dbContext.Students.AddAsync(new Model.Student
                {
                    LastName = "Suppress.Out.002",
                    FirstMidName = "Suppress.Out.002",
                    EnrollmentDate = DateTime.UtcNow,
                });

                await scope.CompleteAsync();//提交事务
            }

            using (var scope = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                MyDbContext _dbContext = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
                await _dbContext.Students.AddAsync(new Model.Student
                {
                    LastName = "Suppress.Out.003",
                    FirstMidName = "Suppress.Out.003",
                    EnrollmentDate = DateTime.UtcNow,
                });

                await _dbContext.SaveChangesAsync();
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
            var a = await _repo.GetAll().IgnoreQueryFilters().ToListAsync();
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
            try
            {
                await _repo.GetAll().ToListAsync();
            }
            catch (Exception e) 
            {

            }
            return Ok();
        }

        #endregion

        #region ado.net
        [HttpPost]
        public async Task<IActionResult> AdoNetInsert() 
        {
            string sql = @"insert into Students(Id,LastName,FirstMidName,EnrollmentDate,TenantId,CreationUserId,IsDeleted) " +
                "values(@Id,@LastName,@FirstMidName,@EnrollmentDate,@TenantId,@CreationUserId,@IsDeleted)";
            MySqlParameter[] sqlParameters = 
            {
                new MySqlParameter("@Id",Guid.NewGuid()),
                new MySqlParameter("@LastName",123),
                new MySqlParameter("@FirstMidName",223),
                new MySqlParameter("@EnrollmentDate",DateTime.Now),
                new MySqlParameter("@TenantId",1),
                new MySqlParameter("@CreationUserId",1),
                new MySqlParameter("@IsDeleted",false),
            };
            var count = await DbExecuter.ExecuteNonQuery(sql, sqlParameters);
            return Ok(count);
        }

        [HttpPost]
        public async Task<IActionResult> AdoNetInsertWithTransaction()
        {
            string sql = @"insert into Students(Id,LastName,FirstMidName,EnrollmentDate,TenantId,CreationUserId,IsDeleted) " +
                "values(@Id,@LastName,@FirstMidName,@EnrollmentDate,@TenantId,@CreationUserId,@IsDeleted)";
            MySqlParameter[] sqlParameters =
            {
                new MySqlParameter("@Id",Guid.NewGuid()),
                new MySqlParameter("@LastName",8899),
                new MySqlParameter("@FirstMidName","aabb1212"),
                new MySqlParameter("@EnrollmentDate",DateTime.Now),
                new MySqlParameter("@TenantId",1),
                new MySqlParameter("@CreationUserId",1),
                new MySqlParameter("@IsDeleted",false),
            };

            using var scope = await DbExecuter.BeginTransactionAsync();
            var count = await DbExecuter.ExecuteNonQuery(sql, sqlParameters);

            string updatesql = $"update Students set LastName = @LastName where FirstMidName = @FirstMidName";
            MySqlParameter[] upsqlParameters = 
            {
                 new MySqlParameter("@LastName",889999),
                 new MySqlParameter("@FirstMidName","aabb1212"),
            };
            count = await DbExecuter.ExecuteNonQuery(updatesql, upsqlParameters);

            await scope.CommitAsync();
            return Ok(count);
        }

        [HttpPost]
        public async Task<IActionResult> AdoNetUpdate() 
        {
            using var scope = await DbExecuter.BeginTransactionAsync();
            var guids = new Guid[] { new Guid("654d3562-37ad-4ff6-8f93-01f988c75fe1") };
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>
            {
                new MySqlParameter("@LastName","最好的克伦克丶"),
            };
            StringBuilder ids = new();
            for (var i = 0; i < guids.Count(); i++)
            {
                ids.Append($"@id{i},");
                sqlParameters.Add(new MySqlParameter($"@id{i}", guids[i]));
            }
            string @id = ids.ToString().TrimEnd(',');
            string sql = $"update Students set LastName = @LastName where id in ({@id})";
            var count = await DbExecuter.ExecuteNonQuery(sql, sqlParameters.ToArray());

            var guids1 = new Guid[] { new Guid("654d3562-37ad-4ff6-8f93-01f988c75fe1") };
            List<MySqlParameter> sqlParameters1 = new List<MySqlParameter>();
            StringBuilder ids1 = new();
            for (var i = 0; i < guids1.Count(); i++)
            {
                ids1.Append($"@id{i},");
                sqlParameters1.Add(new MySqlParameter($"@id{i}", guids[i]));
            }
            string @id1 = ids1.ToString().TrimEnd(',');
            string sql1 = $"select * from Students where id in ({@id1})";
            var datas = await DbExecuter.QueryAsync<Student>(sql1, sqlParameters1.ToArray());

            await scope.CommitAsync();

            return Ok((count,datas));
         }

        [HttpPost]
        public async Task<IActionResult> AdoNetDelete() 
        {
            var guids = new Guid[] { new Guid("654d3562-37ad-4ff6-8f93-01f988c75fe1") };
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
            StringBuilder ids = new();
            for (var i = 0; i < guids.Count(); i++)
            {
                ids.Append($"@id{i},");
                sqlParameters.Add(new MySqlParameter($"@id{i}", guids[i]));
            }
            string @id = ids.ToString().TrimEnd(',');
            string sql = $"delete from Students where id in ({@id})";
            var count = await DbExecuter.ExecuteNonQuery(sql, sqlParameters.ToArray());
            return Ok(count);
        }

        [HttpGet]
        public async Task<IActionResult> AdoNetQuery() 
        {
            var guids = new Guid[] { new Guid("846f3141-53fa-4d49-8b84-d1213fd1d7e1") };
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
            StringBuilder ids = new();
            for (var i = 0; i < guids.Count(); i++)
            {
                ids.Append($"@id{i},");
                sqlParameters.Add(new MySqlParameter($"@id{i}", guids[i]));
            }
            string @id = ids.ToString().TrimEnd(',');
            string sql = $"select * from Students where id in ({@id})";
            var datas = await DbExecuter.QueryAsync<Student>(sql, sqlParameters.ToArray());

            sql = "select count(FirstMidName) as count,FirstMidName from students group by FirstMidName";
            var data2 = await DbExecuter.QueryAsync<object>(sql);

            sql = "select lastname from students group by lastname;";
            var data3 = await DbExecuter.QueryAsync<JObject>(sql);
            var names = data3.Select(r => r.GetValue("lastname")!.ToString());

            return Ok((datas, names));
        }

        [HttpGet]
        public async Task<IActionResult> AdoNetQueryOne()
        {
            var guids = new Guid[] { new Guid("B0C1C8A4-16DD-40F2-862F-79DD0B82F037") };
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
            StringBuilder ids = new();
            for (var i = 0; i < guids.Count(); i++)
            {
                ids.Append($"@id{i},");
                sqlParameters.Add(new MySqlParameter($"@id{i}", guids[i]));
            }
            string @id = ids.ToString().TrimEnd(',');
            string sql = $"select * from Students where id in ({@id})";
            var datas = await DbExecuter.QueryFirstOrDefaultAsync<Student>(sql, sqlParameters.ToArray());
            return Ok(datas);
        }

        [HttpGet]
        public async Task<IActionResult> AdoNetScalar()
        {
            var guids = new Guid[] { new Guid("B0C1C8A4-16DD-40F2-862F-79DD0B82F037"), new Guid("1771F732-B700-4120-8BD9-A39B4654AE72") };
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
            StringBuilder ids = new();
            for (var i = 0; i < guids.Count(); i++)
            {
                ids.Append($"@id{i},");
                sqlParameters.Add(new MySqlParameter($"@id{i}", guids[i]));
            }
            string @id = ids.ToString().TrimEnd(',');
            string sql = $"select count(*) from Students where id in ({@id})";
            var data1 = await DbExecuter.ExecuteScalarAsync<int>(sql, sqlParameters.ToArray());

            return Ok(data1);
        }
        #endregion

        #region operationlog

        [HttpPost]
        public async Task RecordOperationAdd()
        {
            await _repo.InsertAsync(new Student
            {
                FirstMidName = "A001",
                LastName = "A001"
            });

            await _repo.InsertAsync(new Student
            {
                FirstMidName = "A002",
                LastName = "A002"
            });

            await RepoApp.InsertAsync(new AppVersion
            {
                Version = "123",
                Path = "1234"
            });

            await _classRepo.InsertAsync(new ClassRoom
            {
                CreationUserId = 123
            });
        }

        [HttpPost]
        public async Task<string> RecordOperationLogs(string a)
        {
            await _repo.InsertAsync(new Student
            {
                FirstMidName = "C001",
                LastName = "C001"
            });

            var data = await _repo.FirstOrDefaultAsync(r => r.FirstMidName.Equals("A001"));
            data!.LastName = "A001.001";

            var data1 = await _repo.FirstOrDefaultAsync(r => r.FirstMidName.Equals("A002"));
            await _repo.DeleteAsync(data1!);

            var data2 = await RepoApp.GetAll().FirstOrDefaultAsync();
            await RepoApp.DeleteAsync(data2!);

            return "Ok123";
        }

        /// <summary>
        /// 使用ado.net时，efcore-entry集合里面是没有值的，所以获取不到有变更的实体
        /// </summary>
        /// <param name="aa"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> AdoRecordOperationLogs([FromBody] string aa) 
        {
            //DbExecuter.BeginWithEFCoreTransaction(UnitOfWorkManager.Current!);
            //using var scope = await DbExecuter.BeginTransactionAsync();

            string sql = @"insert into Students(Id,LastName,FirstMidName,EnrollmentDate,TenantId,CreationUserId,IsDeleted) " +
               "values(@Id,@LastName,@FirstMidName,@EnrollmentDate,@TenantId,@CreationUserId,@IsDeleted)";
            MySqlParameter[] sqlParameters =
            {
                new MySqlParameter("@Id",Guid.NewGuid()),
                new MySqlParameter("@LastName",8899),
                new MySqlParameter("@FirstMidName","aabb121212"),
                new MySqlParameter("@EnrollmentDate",DateTime.Now),
                new MySqlParameter("@TenantId",1),
                new MySqlParameter("@CreationUserId",1),
                new MySqlParameter("@IsDeleted",false),
            };
            var c1 = await DbExecuter.ExecuteNonQuery(sql, sqlParameters);

            string updatesql = $"update Students set LastName = @LastName where FirstMidName = @FirstMidName";
            MySqlParameter[] upsqlParameters =
            {
                 new MySqlParameter("@LastName","A001.001"),
                 new MySqlParameter("@FirstMidName","A001"),
            };
            var c2 = await DbExecuter.ExecuteNonQuery(updatesql, upsqlParameters);

            string deletesql = "delete from AppVersion where Version = @Version";
            MySqlParameter[] deleteParameters =
            {
                new MySqlParameter("@Version","123"),
            };
            var c3 = await DbExecuter.ExecuteNonQuery(deletesql, deleteParameters);

            deletesql = "delete from Students where FirstMidName = @FirstMidName";
            MySqlParameter[] deleteParameterss =
            {
                new MySqlParameter("@FirstMidName","A002")
            };
            var c4 = await DbExecuter.ExecuteNonQuery(deletesql, deleteParameterss);

            //await scope.CommitAsync();

            return (c1,c2,c3,c4);
        }

        #endregion


    }
}