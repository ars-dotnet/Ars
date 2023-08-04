using Ars.Common.Core.Configs;
using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using Microsoft.AspNetCore.Mvc;

namespace MyApiWithIdentityServer4.Controllers
{
    public abstract class MyControllerBase : ControllerBase, IScopedDependency
    {
        [Autowired]
        public MyDbContext MyDbContext { get; set; }

        [Autowired]
        public ITestDomain TestService { get; set; }

        [Autowired]
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }

        [Autowired]
        public IHttpContextAccessor HttpContextAccessor { get; set; }

        [Autowired]
        public IHttpClientFactory HttpClientFactory { get; set; }

        [Autowired]
        public IArsConfiguration ArsConfiguration { get; set; }
    }
}
