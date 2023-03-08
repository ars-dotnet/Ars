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
        public ArsOutput<GetPlanByNumberOutput> GetPlanByNumber([FromQuery]string PlanNumber)
        {
            return 
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
                    });
        }

        [HttpGet]
        public ArsOutput<IEnumerable<GetPageOutput>> GetPage([FromQuery]GetPageInput input) 
        {
            IEnumerable<GetPageOutput> datas = new List<GetPageOutput>()
            {
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:01",
                    PartBarCode = "P001",
                    DemQty = 100,
                    RealQty = 101,
                    BatchNum = "B001"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200,
                    RealQty = 201,
                    BatchNum = "B002"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200,
                    RealQty = 201,
                    BatchNum = "B002"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200,
                    RealQty = 201,
                    BatchNum = "B002"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200,
                    RealQty = 201,
                    BatchNum = "B002"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200,
                    RealQty = 201,
                    BatchNum = "B002"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200,
                    RealQty = 201,
                    BatchNum = "B002"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200,
                    RealQty = 201,
                    BatchNum = "B002"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200,
                    RealQty = 201,
                    BatchNum = "B002"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200,
                    RealQty = 201,
                    BatchNum = "B002"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200,
                    RealQty = 201,
                    BatchNum = "B002"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200,
                    RealQty = 201,
                    BatchNum = "B002"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200,
                    RealQty = 201,
                    BatchNum = "B002"
                },
            };

            return ArsOutput<IEnumerable<GetPageOutput>>.Success(datas); 
        }
    }
}
