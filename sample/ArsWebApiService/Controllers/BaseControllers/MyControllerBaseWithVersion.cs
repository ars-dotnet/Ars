using Ars.Common.Core.IDependency;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace ArsWebApiService.Controllers.BaseControllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("Api/ArsWebApi/V{version:apiVersion}/[controller]/[action]")]
    public abstract class MyControllerBaseWithVersion : ControllerBase, IScopedDependency
    {

    }
}
