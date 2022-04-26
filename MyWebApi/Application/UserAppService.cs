using Ars.Common.AutoFac;
using Ars.Common.AutoFac.IDependency;
using Ars.Common.Core.IDependency;

namespace MyIdentityWithGithub.Application
{
    public abstract class UserBase : IUserAppService
    {
        public abstract int UserId { get; }

        public abstract string UserName { get; }

        public abstract IEnumerable<string> GetAll();
    }

    public class UserAppService : UserBase
    {
        [Autowired]
        private IHttpContextAccessor httpContextAccessor { get; set; }
        public UserAppService()
        {
            
        }

        public override int UserId => 0;

        public override string UserName => httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "test";

        public override IEnumerable<string> GetAll()
        {
            yield return "tom";
            yield return "tom2";
            yield return "tom3";
        }
    }

    public class TestAppService : ITestAppService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public TestAppService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string UserName => httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "test";
    }

    public class TestAppService1 : ITestAppService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public TestAppService1(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string UserName => httpContextAccessor.HttpContext?.User?.Identity?.Name + "hhh" ?? "test" + "hhh";
    }

    public class Config : ISingletonDependency 
    {
        public Config()
        {
            Age = 11;
        }

        public int Age { get; set; }
    }
}
