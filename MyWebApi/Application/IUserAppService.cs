using Ars.Common.AutoFac.IDependency;

namespace MyIdentityWithGithub.Application
{
    public interface IUserAppService : ITransientDependency,ISingletonDependency
    {
         IEnumerable<string> GetAll();

         int UserId { get; }

         string UserName { get; }
    }

    public interface ITestAppService : IScopedDependency
    {
        string UserName { get; }
    }
}
