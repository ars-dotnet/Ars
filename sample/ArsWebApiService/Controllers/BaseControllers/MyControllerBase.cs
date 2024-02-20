using Ars.Common.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Core.IDependency;
using Ars.Common.Core.Uow;
using ArsWebApiService;
using Microsoft.AspNetCore.Mvc;
using MyApiWithIdentityServer4.Controllers;

namespace ArsWebApiService.Controllers.BaseControllers
{
    public abstract class MyControllerBase : MyControllerBaseWithVersion
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

        [Autowired]
        public IServiceScopeFactory ServiceScopeFactory { get; set; }

        [Autowired]
        public IHttpClientProvider HttpClientProvider { get; set; }
    }
}
