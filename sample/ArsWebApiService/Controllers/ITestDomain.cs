using Ars.Common.Core.IDependency;

namespace MyApiWithIdentityServer4.Controllers
{
    public interface ITestDomain : IScopedDependency
    {
        Task Test();
    }

    public abstract class BaseTestDomain : ITestDomain
    {
        [Autowired]
        public ITestService TestService { get; set; }

        public BaseTestDomain()
        {
            
        }

        public abstract Task Test();
    }

    public class TestDomain : BaseTestDomain
    {
        private IServiceScopeFactory _serviceScopeFactory;
        public TestDomain(IServiceScopeFactory serviceScopeFactory)
        {
            this._serviceScopeFactory = serviceScopeFactory;
        }

        public override Task Test() 
        {
            var code = TestService.GetHashCode();
            return Task.CompletedTask;
        }
    }
}
