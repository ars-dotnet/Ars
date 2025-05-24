using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.Configs;
using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Ars.Common.Core.Uow.Attributes;
using Ars.Common.EFCore.AdoNet;
using Ars.Common.EFCore.Extension;
using Ars.Common.EFCore.Repository;
using ArsWebApiService;
using ArsWebApiService.Controllers.BaseControllers;
using ArsWebApiService.Model;
using ArsWebApiService.Services;
using Asp.Versioning;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MyApiWithIdentityServer4.Dtos;
using MyApiWithIdentityServer4.Model;
using MySqlConnector;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SkyWalking.NetworkProtocol.V3;
using System.Text;
using System.Transactions;

namespace MyApiWithIdentityServer4.Controllers
{
    /// <summary>
    /// dbcontext test controller
    /// </summary>
    public class DbContextController : MyControllerBase
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly ILogger<DbContextController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly ITestScopeService _testScopeService;
        private readonly IArsIdentityClientConfiguration _clientConfiguration;
        //private readonly MyDbContext myDbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IServiceProvider _serviceProvider;
        private readonly IOptions<IArsBasicConfiguration> _options;
        private readonly IArsConfiguration _arsConfiguration;
        public DbContextController(ILogger<DbContextController> logger,
            MyDbContext myDbContext,
            IHttpClientFactory httpClientFactory,
            IHttpContextAccessor httpContextAccessor,
            //ITestScopeService testScopeService,
            IArsIdentityClientConfiguration arsIdentityClientConfiguration,
            IUnitOfWork unitOfWork,
            IServiceProvider serviceProvider,
            IOptions<IArsBasicConfiguration> options,
            IArsConfiguration arsConfiguration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            //this.myDbContext = myDbContext;
            _httpContextAccessor = httpContextAccessor;
            //_testScopeService = testScopeService;
            _clientConfiguration = arsIdentityClientConfiguration;
            _unitOfWork = unitOfWork;
            _serviceProvider = serviceProvider;
            _options = options;
            _arsConfiguration = arsConfiguration;
        }

        [Autowired]
        public IRepository<Student, Guid> Repo { get; set; }

        [Autowired]
        public IRepository<Enrollment> EnrollmentRepo { get; set; }

        [Autowired]
        public IRepository<MyDbContext2,StudentNew, Guid> StuNew_Repo { get; set; }

        [Autowired]
        public IRepository<StudentMsSql, Guid> Repo1 { get; set; }

        [Autowired]
        public IRepository<ClassRoom, Guid> ClassRepo { get; set; }

        [Autowired]
        public IDbExecuter<MyDbContext> DbExecuter { get; set; }

        [Autowired]
        public IDbExecuter<MyDbContext2> DbExecuter2 { get; set; }

        [Autowired]
        protected IRepository<AppVersion> RepoApp { get; set; }

        [Autowired]
        public IService Service { get; set; }

        [Autowired]
        public IMService MService { get; set; }

        #region DbContext with Default Transaction

        [HttpPost(nameof(ActionWithDefaultTransaction))]
        public async Task ActionWithDefaultTransaction()
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
                        Id = 3,
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
        public async Task<Student?> Query()
        {
            var q = from a in Repo.GetAll()
                    join b in StuNew_Repo.GetAll()
                    on a.LastName equals b.Name
                    select a;

            var data = await q.FirstOrDefaultAsync();

            return data;
        }

        [Authorize("default")]
        [HttpPost(nameof(ModifyWithDefaultTransaction))]
        public async Task ModifyWithDefaultTransaction()
        {
            var info = await MyDbContext.Students.FirstOrDefaultAsync();
            info.LastName = "boo" + new Random().Next(20);

            await MyDbContext.SaveChangesAsync();
        }

        [Authorize]
        [HttpPost(nameof(DeleteWithDefaultTransaction))]
        public async Task DeleteWithDefaultTransaction()
        {
            var info = await MyDbContext.Students.FirstOrDefaultAsync();
            MyDbContext.Students.Remove(info);

            await MyDbContext.SaveChangesAsync();
        }



        [HttpPost]
        [UnitOfWork(IsDisabled = true)]
        public async Task<IActionResult> TestDefaultUOW()
        {
            using var scope = UnitOfWorkManager.Begin();
            var info = await Repo.FirstOrDefaultAsync(r => r.LastName.Equals("TestUowDefault11"));
            info!.LastName = "TestUowDefault12";

            var a = await Repo.CountAsync(r => r.LastName.Equals("TestUowDefault11"));

            await UnitOfWorkManager.Current.SaveChangesAsync();

            a = await Repo.CountAsync(r => r.LastName.Equals("TestUowDefault11"));
            await scope.CompleteAsync();
            return Ok(a);
        }

        #endregion

        #region DbContext with Custome Transaction


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
            var a = await Repo.GetAll().FirstOrDefaultAsync();

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
        public async Task<IActionResult> GetOneAsync()
        {
            var data = await MService.GetAsync();

            return Ok(data);
        }

        /// <summary>
        /// 测试mysql 可重复读的事务隔离级别
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDataByRR()
        {
            #region 多次读，即使另一个事务update,insert,delete了相同条件的数据，当前事务读取到的数据未变 
            var query = Repo.GetAll().Where(r => r.LastName == "123");

            var data = await query.ToListAsync();

            // * * * * *
            //此时另一个事务insert了两条数据
            // * * * * *

            await Task.Delay(100);

            data = await query.ToListAsync();

            await Task.Delay(100);

            data = await query.ToListAsync();

            #endregion

            #region 当前事务的update操作，会将另一个事务insert的数据也给update掉

            DbExecuter.BeginWithEFCoreTransaction(UnitOfWorkManager.Current!);
            string updatesql = $"update Students set FirstMidName = @FirstMidName where LastName = @LastName";
            MySqlParameter[] upsqlParameters =
            {
                 new MySqlParameter("@LastName","123"),
                 new MySqlParameter("@FirstMidName","早上好12"),
            };
            await DbExecuter.ExecuteNonQuery(updatesql, upsqlParameters);

            #endregion

            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = Repo.GetAll().Where(r => r.LastName == "TestUowRequired");
            var sql = query.ToQueryString();

            var a = await Repo.GetAll().IgnoreQueryFilters().ToListAsync();
            var b = await Repo.GetAllIncluding(r => r.Enrollments).ToListAsync();
            var c = Repo.GetAllList();
            var d = Repo.GetAllList(r => r.Enrollments.Any(t => t.Id == 1));
            var e = Repo.FirstOrDefault(r => r.Id == new Guid("8FB45ADF-3F80-45ED-93CB-10A61CE644E9"));

            var m = _options.Value.ServiceIp;

            var n = _arsConfiguration;

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> InsertWithIdAsync()
        {
            Guid id = Guid.NewGuid();
            var f = await Repo.InsertAsync(new Student
            {
                Id = id,
                EnrollmentDate = DateTime.Now,
                FirstMidName = "7777",
                LastName = "77778",
                Enrollments = new[]
                {
                    new Model.Enrollment
                    {
                        Id = 6,
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
            var f = await Repo.InsertAsync(new Student
            {
                EnrollmentDate = DateTime.Now,
                FirstMidName = "6666",
                LastName = "6666",
            });

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync()
        {
            var e = await Repo.FirstOrDefaultAsync(r => r.Id == new Guid("8FB45ADF-3F80-45ED-93CB-10A61CE644E9"));
            e.LastName = "8888";

            foreach (var en in e.Enrollments)
            {
                en.Grade = Grade.C;
                en.Course.Name = "8888";
            }

            await Repo.UpdateAsync(e);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync()
        {
            var h = await Repo.FirstOrDefaultAsync(r => r.Id == new Guid("CAEF9CEF-EBA3-47DA-AAF9-CF2802413F97"));
            await Repo.DeleteAsync(h);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsyncTest()
        {
            var a = await (await Repo.GetAllAsync()).ToListAsync();
            var b = await (await Repo.GetAllIncludingAsync(r => r.Enrollments)).ToListAsync();
            var c = await Repo.GetAllListAsync();
            var d = await Repo.GetAllListAsync(r => r.Enrollments.Any(t => t.Id == 1));
            var e = await Repo.FirstOrDefaultAsync(r => r.Id == new Guid("8FB45ADF-3F80-45ED-93CB-10A61CE644E9"));

            return Ok(a);
        }

        [HttpGet]
        [UnitOfWork(IsDisabled = true)]
        public async Task<IActionResult> GetWithOutTransaction()
        {
            try
            {
                var aa = await Repo.FirstOrDefaultAsync(r => r.LastName.Equals("6666"));
                aa!.FirstMidName = "6667";

                var aaa = await Repo.FirstOrDefaultAsync(r => r.FirstMidName.Equals("aabb121211"));
                aaa!.FirstMidName = "66678";

                var manager = ServiceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
                using var scope = manager.Begin();

                var repo = ServiceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IRepository<Student, Guid>>();
                {
                    var a = await repo.FirstOrDefaultAsync(r => r.LastName.Equals("6666"));
                    a.LastName = "12345";
                }

                await scope.CompleteAsync();
            }
            catch (Exception e)
            {

            }
            return Ok();
        }

        #endregion

        #region ado.net

        [HttpPost(nameof(TestUowRequiredNew))]
        public async Task TestUowRequiredNew()
        {
            using var scope1 = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew);
            MyDbContext _dbContext = await UnitOfWorkManager.Current.GetDbContextAsync<MyDbContext>();
            await _dbContext.Students.AddAsync(new Model.Student
            {
                LastName = "TestUowRequiredNew",
                FirstMidName = "TestUowRequiredNew",
                EnrollmentDate = DateTime.Now,
            });

            //ado.net使用efcore的事务
            DbExecuter.BeginWithEFCoreTransaction(UnitOfWorkManager.Current!);

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
            var count = await DbExecuter.ExecuteNonQuery(sql, sqlParameters);

            string updatesql = $"update Students set LastName = @LastName where FirstMidName = @FirstMidName";
            MySqlParameter[] upsqlParameters =
            {
                 new MySqlParameter("@LastName",889999),
                 new MySqlParameter("@FirstMidName","aabb121212"),
            };
            count = await DbExecuter.ExecuteNonQuery(updatesql, upsqlParameters);

            //新的dbcontext,也使用efcore的事务
            DbExecuter2.BeginWithEFCoreTransaction(UnitOfWorkManager.Current!);

            sql = @"insert into StudentNew(Id,Name,TenantId,CreationUserId,IsDeleted) " +
                "values(@Id,@Name,@TenantId,@CreationUserId,@IsDeleted)";
            MySqlParameter[] sqlParameterss =
            {
                new MySqlParameter("@Id",Guid.NewGuid()),
                new MySqlParameter("@Name", "aabb121212"),
                new MySqlParameter("@TenantId",1),
                new MySqlParameter("@CreationUserId",1),
                new MySqlParameter("@IsDeleted",false),
            };
            count = await DbExecuter2.ExecuteNonQuery(sql, sqlParameterss);

            await scope1.CompleteAsync();
        }

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
        [UnitOfWork(IsDisabled = true)]
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
            var guids = new Guid[]
            {
                new Guid("9dc35d1c-da51-4a6d-a3be-df299e2fa88a"),
                new Guid("b0d7a84f-c0f2-42f3-964c-93ff84ca47c4")
            };
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

            var guids1 = new Guid[] {
                new Guid("9dc35d1c-da51-4a6d-a3be-df299e2fa88a"),
                new Guid("b0d7a84f-c0f2-42f3-964c-93ff84ca47c4")
            };
            List<MySqlParameter> sqlParameters1 = new List<MySqlParameter>();
            StringBuilder ids1 = new();
            for (var i = 0; i < guids1.Count(); i++)
            {
                ids1.Append($"@id{i},");
                sqlParameters1.Add(new MySqlParameter($"@id{i}", guids[i]));
            }
            string @id1 = ids1.ToString().TrimEnd(',');
            string sql1 = $"select * from Students where id in ({@id1})";

            //这里读取时，是修改后的值
            var datas = await DbExecuter.QueryAsync<Student>(sql1, sqlParameters1.ToArray());

            await scope.CommitAsync();

            return Ok((count, datas));
        }

        [HttpPost]
        public async Task<IActionResult> AdoNetDelete()
        {
            var guids = new Guid[] { new Guid("9dc35d1c-da51-4a6d-a3be-df299e2fa88a") };
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

            sqlParameters = new List<MySqlParameter>()
            {
                new MySqlParameter("@lastname","%boo%")
            };
            sql = "select * from students where lastname like @lastname;";
            var data4 = await DbExecuter.QueryAsync<JObject>(sql, sqlParameters.ToArray());

            return Ok((datas, names, data4));
        }

        [HttpGet]
        public async Task<IActionResult> AdoNetQueryOne()
        {
            var guids = new Guid[] { new Guid("b0d7a84f-c0f2-42f3-964c-93ff84ca47c4") };
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
            var guids = new Guid[]
            {
                new Guid("05db0cc6-8f4d-4d15-b3d0-21ac0dc73335"),
                new Guid("08db60b5-0b18-4526-89bf-6d131c6be055")
            };
            List<MySqlParameter> sqlParameters = new List<MySqlParameter>();
            StringBuilder ids = new();
            for (var i = 0; i < guids.Count(); i++)
            {
                ids.Append($"@id{i},");
                sqlParameters.Add(new MySqlParameter($"@id{i}", guids[i]));
            }
            string @id = ids.ToString().TrimEnd(',');
            string sql = $"select count(*) from Students where id in ({@id})";
            var data1 = await DbExecuter.ExecuteScalarAsync<long>(sql, sqlParameters.ToArray());

            return Ok(data1);
        }
        #endregion

        #region operationlog

        [HttpPost]
        public async Task RecordOperationAdd()
        {
            await Repo.InsertAsync(new Student
            {
                FirstMidName = "A001",
                LastName = "A001"
            });

            await Repo.InsertAsync(new Student
            {
                FirstMidName = "A002",
                LastName = "A002"
            });

            await RepoApp.InsertAsync(new AppVersion
            {
                Version = "123",
                Path = "1234"
            });

            await ClassRepo.InsertAsync(new ClassRoom
            {
                CreationUserId = 123
            });
        }

        [HttpPost]
        public async Task<string> RecordOperationLogs(string a)
        {
            await Repo.InsertAsync(new Student
            {
                FirstMidName = "C001",
                LastName = "C001"
            });

            var data = await Repo.FirstOrDefaultAsync(r => r.FirstMidName.Equals("A001"));
            data!.LastName = "A001.001";

            var data1 = await Repo.FirstOrDefaultAsync(r => r.FirstMidName.Equals("A002"));
            await Repo.DeleteAsync(data1!);

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

            return (c1, c2, c3, c4);
        }

        #endregion

        #region Multiple data sources

        /// <summary>
        /// 多数据源测试
        /// 不同dbcontext，相同数据库相同连接字符串 [dbcontexts共用一个事务]
        /// 不同dbcontext，不同数据库 [dbcontexts不共用一个事务]
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [UnitOfWork(IsDisabled = true)]
        public async Task<IActionResult> MultipleDataSource()
        {
            using var scope = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew);

            //mysql
            var data = await Repo.FirstOrDefaultAsync(r => r.Id == Guid.Parse("05db0cc6-8f4d-4d15-b3d0-21ac0dc73335"));
            data!.LastName = DateTime.Now.ToString("yyyyMMddHHmmss");

            var datax = await Repo.FirstOrDefaultAsync(r => r.Id == Guid.Parse("08db60b5-0b18-4526-89bf-6d131c6be055"));
            datax!.LastName = DateTime.Now.ToString("yyyyMMddHHmmss");

            //mysql - new dbcontext
            await StuNew_Repo.InsertAsync(new StudentNew
            {
                Name = DateTime.Now.ToString("yyyyMMddHHmmss")
            });

            //mssql
            var data2 = await Repo1.FirstOrDefaultAsync(r => r.Id == Guid.Parse("55AF24DD-25CF-49D7-6B61-08DB56996478"));
            data2!.LastName = DateTime.Now.ToString("yyyyMMddHHmmss");

            //using var scope = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew);

            //var data1 = await Repo.FirstOrDefaultAsync(r => r.Id == Guid.Parse("05db0cc6-8f4d-4d15-b3d0-21ac0dc73336"));
            //data1!.LastName = DateTime.Now.ToString("yyyyMMddHHmmss");

            //await StuNew_Repo.InsertAsync(new StudentNew
            //{
            //    Name = DateTime.Now.ToString("yyyyMMddHHmmss")
            //});

            await scope.CompleteAsync();

            return Ok((data, data2));
        }

        #endregion

        /// <summary>
        /// 测试efcore延迟加载
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UnitOfWork(IsDisabled = true)]
        public async Task<IActionResult> TestUseLazyLoadingProxies()
        {
            IEnumerable<Task> tasks = new List<Task>
            {
                TestUseLazyLoadingProxie(120,121),
                TestUseLazyLoadingProxie(122,123),
                TestUseLazyLoadingProxie(124,125),
                TestUseLazyLoadingProxie(126,127),
                TestUseLazyLoadingProxie(128,129),

                TestUseLazyLoadingProxie(130,131),
                TestUseLazyLoadingProxie(132,133),
                TestUseLazyLoadingProxie(134,135),
                TestUseLazyLoadingProxie(136,137),
                TestUseLazyLoadingProxie(138,139),
            };

            await Task.WhenAll(tasks);

            return Ok();
        }

        private async Task TestUseLazyLoadingProxie(int id1, int id2)
        {
            using var scope = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew);

            Guid id = Guid.NewGuid();
            await Repo.InsertAsync(new Student
            {
                Id = id,
                LastName = "ars bill",
                FirstMidName = "ars bill",
                EnrollmentDate = DateTime.Now,
                Enrollments = new List<Enrollment>
                {
                    new Enrollment
                    {
                        Id = id1,
                        CourseID = 6,
                        StudentID = id,
                    },
                    new Enrollment
                    {
                        Id = id2,
                        CourseID = 6,
                        StudentID = id,
                    },
                },
            });

            await scope.CompleteAsync();
        }

        [HttpGet]
        [UnitOfWork(IsDisabled = true)]
        public async Task<IActionResult> TestIRepository()
        {
            var data = await Repo.FirstOrDefaultAsync();

            var data1 = await StuNew_Repo.FirstOrDefaultAsync();

            //Cannot use multiple context instances within a single query execution.
            //Ensure the query uses a single context instance.

            //var data2 = await (from a in Repo.GetAll()
            //                   join b in EnrollmentRepo.GetAll()
            //                   on a.Id equals b.StudentID
            //                   select new { a,b } ).FirstOrDefaultAsync();

            Guid id = Guid.NewGuid();

            await Repo.InsertAsync(new Student
            {
                Id = id,
                LastName = "ars bill",
                FirstMidName = "ars bill",
                EnrollmentDate = DateTime.Now,
                Enrollments = new List<Enrollment>
                {
                    new Enrollment
                    {
                        Id = 140,
                        CourseID = 6,
                        StudentID = id,
                    },
                    new Enrollment
                    {
                        Id = 141,
                        CourseID = 6,
                        StudentID = id,
                    },
                },
            });

            //不同的dbcontext
            await Repo.SaveChangesAsync();

            await StuNew_Repo.InsertAsync(new StudentNew
            {
                Name = "Bill"
            });

            //不同的dbcontext
            await StuNew_Repo.SaveChangesAsync();

            //开发时不建议在不使用unit of work的情况下，对多个表进行更改
            //无法做到原子操作

            return Ok((data, data1));
        }

        /// <summary>
        /// 测试相同表名放在不同的数据库上下文
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UnitOfWork(IsDisabled = true)]
        public async Task<IActionResult> TestSameTableNameWithDiffDbContext(
            [FromServices] IRepository<MyDbContext, StudentNew, Guid> repo2,
            [FromServices] IRepository<MyDbContext2, StudentNew, Guid> repo3)
        {
            using var scope = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew);

            await repo2.InsertAsync(new StudentNew 
            {
                Name = "haha"
            });

            await repo3.InsertAsync(new StudentNew
            {
                Name = "hahaha"
            });

            await scope.CompleteAsync();

            return Ok();
        }

        [HttpPost]
        [UnitOfWork(IsDisabled = true)]
        public async Task<IActionResult> TestMultipleUpdateSameData([FromServices] IRepository<MyDbContext2, StudentNew, Guid> repo3)
        {
            IEnumerable<Task> tasks = new List<Task>
            {
                Func1(repo3),
                Func2(repo3),
            };

            await Task.WhenAll(tasks);

            return Ok();
        }

        private async Task Func1(IRepository<MyDbContext2, StudentNew, Guid> repo3)
        {
            for (int i = 0;i < 20;i++) 
            {
                using var scope = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew);

                var data = await repo3.FirstOrDefaultAsync();

                data.Age += 1;

                await scope.CompleteAsync();
            }
        }

        private async Task Func2(IRepository<MyDbContext2, StudentNew, Guid> repo3)
        {
            for (int i = 0; i < 20; i++)
            {
                using var scope = UnitOfWorkManager.Begin(TransactionScopeOption.RequiresNew);

                var data = await repo3.FirstOrDefaultAsync();

                data.Age2 += 1;

                await scope.CompleteAsync();
            }
        }

        [HttpPost]
        public async Task<IActionResult> TestSaveChanges() 
        {
            AppVersion studentNew = new AppVersion()
            {
                Version = "1111",
                Path = "111111"
            };

            await RepoApp.InsertAsync(studentNew);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> TestSaveChanges123()
        {
            AppVersion? studentNew = await RepoApp.GetAll()
                .Where(r => r.Version.Equals("12345"))
                .OrderByDescending(r => r.CreationTime)
                .FirstOrDefaultAsync();

            if (null != studentNew) 
            {
                studentNew.Path = "aabb121233";
            }

            studentNew = new AppVersion()
            {
                Version = "12345",
                Path = "12345"
            };

            await RepoApp.InsertAsync(studentNew);

            studentNew = new AppVersion()
            {
                Version = "123456",
                Path = "123456"
            };

            await RepoApp.InsertAsync(studentNew);

            await UnitOfWorkManager.Current.SaveChangesAsync();

            return Ok();
        }
    }
}