using Ars.Common.Core.AspNetCore.HostService;
using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Ars.Common.EFCore.Repository;
using Microsoft.EntityFrameworkCore;
using MyApiWithIdentityServer4.Model;
using Newtonsoft.Json;

namespace ArsWebApiService.ManualHostService
{
    public class ManualHost2Service : ArsBaseManualExecutingService
    {
        private int i;

        [Autowired]
        protected IRepository<Student, Guid> Repo { get; set; }

        [Autowired]
        protected IServiceScopeFactory ScopeFactory { get; set; }

        private JsonSerializerSettings _jsonSerializerSettings;

        public ManualHost2Service(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss.fff",       //指定时间格式
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore //忽略循环引用，默认是throw exception
            };
        }

        protected override TimeSpan DueTime => TimeSpan.FromSeconds(10);

        protected override TimeSpan Period => Timeout.InfiniteTimeSpan;

        protected override async Task ExecutingAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref i);
            _logger.LogInformation($"{nameof(ManualHost2Service)}.{nameof(ExecutingAsync)}.{i}");

            //using var scope = ScopeFactory.CreateScope();
            //var manager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
            //using var trans = manager.Begin();
            //var repo = scope.ServiceProvider.GetRequiredService<IRepository<Student, Guid>>();
            var a = await Repo.GetAll().Include(r => r.Enrollments).ThenInclude(r => r.Course).FirstOrDefaultAsync(r => r.LastName.Equals("Yang"));

            _logger.LogInformation($"{nameof(ManualHost2Service)}.{nameof(ExecutingAsync)}.{JsonConvert.SerializeObject(a, _jsonSerializerSettings)}");

            //await trans.CompleteAsync();
            return;
        }

        protected override Task StopExecutedAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(ManualHost2Service)}.{nameof(StopExecutedAsync)}.{i}");

            return Task.CompletedTask;
        }

        protected override Task FailedExecutingAsync(Exception e, CancellationToken cancellationToken)
        {
            _logger.LogError($"{nameof(ManualHost2Service)}.{nameof(FailedExecutingAsync)}.{i}");

            return Task.CompletedTask;
        }
    }
}
