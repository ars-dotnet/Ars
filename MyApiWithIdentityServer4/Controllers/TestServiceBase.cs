using Ars.Common.Core.IDependency;

namespace MyApiWithIdentityServer4.Controllers
{
    public abstract class TestServiceBase
    {
        [Autowired]
        public MyDbContext myDbContext { get; set; }
    }
}
