using Ars.Common.Core.IDependency;

namespace MyApiWithIdentityServer4.Controllers
{
    public interface ITestDomain
    {
        Task Test();
    }

    public class TestDomain : ITestDomain ,ISingletonDependency
    {
        private ITestService testService;
        private IServiceScopeFactory _serviceScopeFactory;
        public TestDomain(ITestService testService, IServiceScopeFactory serviceScopeFactory)
        {
            int code = testService.GetHashCode();
            this.testService = testService;
            this._serviceScopeFactory = serviceScopeFactory;
        }

        public Task Test() 
        {
            using (var scope = _serviceScopeFactory.CreateScope()) 
            {
                var m = scope.ServiceProvider.GetService<ITestService>();
                int code = m.GetHashCode();
            }
            return Task.CompletedTask;
        }
    }
}
