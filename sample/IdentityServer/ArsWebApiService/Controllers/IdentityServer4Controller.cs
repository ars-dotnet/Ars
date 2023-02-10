using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ArsWebApiService.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IdentityServer4Controller : Controller
    {
        [Authorize]
        [HttpGet]
        public IActionResult GetClaims()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
