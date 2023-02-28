using Ars.Common.Core.AspNetCore.OutputDtos;
using ArsWebApiService.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ArsWebApiService.Controllers.H5Controllers
{
    [ApiController]
    [Route("Api/[controller]/[action]")]
    [Authorize("default")]
    public class H5PlanController : ControllerBase
    {
        [HttpGet]
        public Task<ArsOutput<GetPlanByNumberOutput>> GetPlanByNumber([FromQuery]string PlanNumber)
        {
            return Task.FromResult(
                ArsOutput<GetPlanByNumberOutput>.Success(
                    new GetPlanByNumberOutput 
                    {
                        PlanNumber = PlanNumber,
                        OrderNum = "O123",
                        SnNum = "S123",
                        CusName = "C123",
                        ProductName = "苹果",
                        BarCode = "B123",
                        BomNumber = "Bom123",
                        PlanQty = 666
                    }));
        }
    }
}
