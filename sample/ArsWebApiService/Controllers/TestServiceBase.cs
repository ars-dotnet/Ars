using Ars.Common.Core.IDependency;
using ArsWebApiService;

namespace MyApiWithIdentityServer4.Controllers
{
    public abstract class TestServiceBase
    {
        [Autowired]
        public MyDbContext myDbContext { get; set; }
    }
}
