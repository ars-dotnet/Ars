using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Microsoft.AspNetCore.Mvc;

namespace MyApiWithIdentityServer4.Controllers
{
    public abstract class MyControllerBase : ControllerBase, ITransientDependency
    {
        [Autowired]
        public MyDbContext MyDbContext { get; set; }

        [Autowired]
        public ITestDomain TestService { get; set; }

        [Autowired]
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }
    }
}
