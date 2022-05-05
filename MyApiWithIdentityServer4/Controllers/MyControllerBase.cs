using Ars.Common.Core.IDependency;
using Microsoft.AspNetCore.Mvc;

namespace MyApiWithIdentityServer4.Controllers
{
    public abstract class MyControllerBase : ControllerBase,ITransientDependency
    {
        [Autowired]
        public MyDbContext myDbContext { get; set; }

        [Autowired]
        public ITestDomain testService { get; set; }
    }
}
