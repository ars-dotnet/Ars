using Ars.Common.Core.AspNetCore.HostService;
using Ars.Common.Core.IDependency;
using Ars.Common.EFCore.Repository;
using MyApiWithIdentityServer4.Model;

namespace ArsWebApiService.StartupHostServices
{
    public class Host1Service : ArsBaseHostStartupExecutingService
    {
        private int i;
        public Host1Service(ILoggerFactory loggerFactory) : base(loggerFactory)
        {

        }

        protected override TimeSpan DueTime => TimeSpan.FromSeconds(10);

        protected override TimeSpan Period => Timeout.InfiniteTimeSpan;

        [Autowired]
        protected IRepository<Student, Guid> Repo { get; set; }

        protected override async Task ExecutingAsync(CancellationToken cancellationToken)
        {
            //var info = await Repo.FirstOrDefaultAsync();


        }
    }
}
