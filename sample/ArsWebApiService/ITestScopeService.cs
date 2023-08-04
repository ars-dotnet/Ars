using MyApiWithIdentityServer4.Controllers;

namespace MyApiWithIdentityServer4
{
    public interface ITestScopeService
    {

    }

    public class TestScopeService : ITestScopeService 
    {
        private readonly ITestService _testDomain;
        public TestScopeService(ITestService testDomain)
        {
            int code = testDomain.GetHashCode();
            _testDomain = testDomain;
        }
    }
}
