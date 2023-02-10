using Ars.Common.Core.IDependency;

namespace MyApiWithIdentityServer4.Controllers
{
    public interface ITestService
    {


    }

    public class TestService : TestServiceBase, ITestService, IScopedDependency
    {
        public TestService()
        {

        }
    }
}
