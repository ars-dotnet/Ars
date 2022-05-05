using Ars.Common.Core.IDependency;

namespace MyApiWithIdentityServer4.Controllers
{
    public interface ITestDomain
    {
        Task Test();
    }

    public class TestDomain : ITestDomain ,ITransientDependency
    {
        private ITestService testService;
        public TestDomain(ITestService testService)
        {
            this.testService = testService;
        }

        public Task Test() 
        {
            return Task.CompletedTask;
        }
    }
}
