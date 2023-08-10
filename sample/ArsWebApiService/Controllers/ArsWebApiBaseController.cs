using Microsoft.AspNetCore.Mvc;

namespace ArsWebApiService.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("Api/ArsWebApi/[controller]/[action]")]
    public abstract class ArsWebApiBaseController : ControllerBase
    {

    }
}
