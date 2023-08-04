using Ars.Common.Core.AspNetCore.HostService;
using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Ars.Common.Core.Uow.Options;
using Ars.Common.EFCore.Repository;
using ArsWebApiService.StartupHostServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using MyApiWithIdentityServer4.Model;
using Newtonsoft.Json;

namespace ArsWebApiService.ManualHostService
{
    public class ManualHost1Service : ArsBaseManualExecutingService
    {
        private int i;

        [Autowired]
        protected IRepository<Student, Guid> Repo { get; set; }

        [Autowired]
        protected IServiceScopeFactory ScopeFactory { get; set; }

        private JsonSerializerSettings _jsonSerializerSettings;

        public ManualHost1Service(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _jsonSerializerSettings = new JsonSerializerSettings()
            {
                DateFormatString = "yyyy-MM-dd HH:mm:ss.fff",       //指定时间格式
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore //忽略循环引用，默认是throw exception
            };
        }

        protected override TimeSpan DueTime => TimeSpan.FromSeconds(10);

        protected override TimeSpan Period => TimeSpan.FromSeconds(30);

        protected override async Task ExecutingAsync(CancellationToken cancellationToken)
        {
            Interlocked.Increment(ref i);
            _logger.LogInformation($"{nameof(ManualHost1Service)}.{nameof(ExecutingAsync)}.{i}");

            //using var scope = ScopeFactory.CreateScope();
            //var manager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();
            //using var trans = manager.Begin(new UnitOfWorkOptions { IsTransactional = false});
            //var repo = scope.ServiceProvider.GetRequiredService<IRepository<Student, Guid>>();

            var a = await (await Repo.GetAllAsync()).Include(r => r.Enrollments).ThenInclude(r => r.Course).FirstOrDefaultAsync(r => r.LastName.Equals("Yang"));

            _logger.LogInformation($"{nameof(ManualHost1Service)}.{nameof(ExecutingAsync)}.{JsonConvert.SerializeObject(a, _jsonSerializerSettings)}");

            //await trans.CompleteAsync();
            return ;
        }

        protected override Task StopExecutedAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(ManualHost1Service)}.{nameof(StopExecutedAsync)}.{i}");

            return Task.CompletedTask;
        }

        protected override Task FailedExecutingAsync(Exception e, CancellationToken cancellationToken)
        {
            _logger.LogError($"{nameof(ManualHost1Service)}.{nameof(FailedExecutingAsync)}.{i}");

            return Task.CompletedTask;
        }
    }
}
