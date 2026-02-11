using Ars.Common.Core.IDependency;

namespace MyApiWithIdentityServer4.Controllers
{
    public interface ITestDomain : IScopedDependency
    {
        Task<string> Test();
    }

    public abstract class BaseTestDomain : ITestDomain
    {
        [Autowired]
        public ITestService TestService { get; set; }

        public BaseTestDomain()
        {
            
        }

        public abstract Task<string> Test();
    }

    [KeyedService("Bird")]
    public class BirdDomain : BaseTestDomain
    {
        private IServiceScopeFactory _serviceScopeFactory;

        public BirdDomain(IServiceScopeFactory serviceScopeFactory)
        {
            this._serviceScopeFactory = serviceScopeFactory;
        }

        public override Task<string> Test()
        {
            return Task.FromResult("叽叽喳喳");
        }
    }

    [KeyedService("Cat")]
    public class CatDomain : BaseTestDomain
    {
        private IServiceScopeFactory _serviceScopeFactory;
        public CatDomain(IServiceScopeFactory serviceScopeFactory)
        {
            this._serviceScopeFactory = serviceScopeFactory;
        }

        public override Task<string> Test()
        {
            return Task.FromResult("喵喵");
        }
    }

    [KeyedService("Dog")]
    public class DogDomain : BaseTestDomain
    {
        private IServiceScopeFactory _serviceScopeFactory;
        public DogDomain(IServiceScopeFactory serviceScopeFactory)
        {
            this._serviceScopeFactory = serviceScopeFactory;
        }

        public override Task<string> Test()
        {
            return Task.FromResult("汪汪");
        }
    }
}
