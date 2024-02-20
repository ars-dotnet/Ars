using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.Excels.ExportExcel;
using ArsWebApiService.Controllers.BaseControllers;
using ArsWebApiService.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ArsWebApiService.Controllers.H5Controllers
{
    [ExportController]
    public class H5PlanController : ArsWebApiBaseController
    {
        [ExportAction]
        [HttpGet]
        public async Task<ArsOutput<GetPlanByNumberOutput>> GetPlanByNumber([FromQuery]string PlanNumber)
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

        [ExportAction]
        [HttpGet]
        public Task<ArsOutput<GetPlanByNumberOutput>> GetPlan() 
        {
            return Task.FromResult(
                ArsOutput<GetPlanByNumberOutput>.Success(
                    new GetPlanByNumberOutput
                    {
                        PlanNumber = "1234",
                        OrderNum = "O123",
                        SnNum = "S123",
                        CusName = "C123",
                        ProductName = "苹果",
                        BarCode = "B123",
                        BomNumber = "Bom123",
                        PlanQty = 888
                    }));
        }

        [ExportAction]
        [HttpGet]
        public Task<ArsOutput<GetPlanByNumberOutput>> GetPlans(string A,string B,int C)
        {
            return Task.FromResult(
                ArsOutput<GetPlanByNumberOutput>.Success(
                    new GetPlanByNumberOutput
                    {
                        PlanNumber = "1234",
                        OrderNum = "O123",
                        SnNum = "S123",
                        CusName = "C123",
                        ProductName = "苹果",
                        BarCode = "B123",
                        BomNumber = "Bom123",
                        PlanQty = 999
                    }));
        }

        [ExportAction]
        [HttpGet]
        public ArsOutput<IEnumerable<GetPageOutput>> GetPage([FromQuery]GetPageInput input) 
        {
            string a = JsonConvert.SerializeObject(input);

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

        [ExportAction]
        [HttpGet]
        public IEnumerable<GetPageOutput> GetPages([FromQuery] Search<DataInput> input, [FromQuery] DataInputs dataInputs) 
        {
            var a = JsonConvert.SerializeObject(input);
            var b = JsonConvert.SerializeObject(dataInputs);

            IEnumerable<GetPageOutput> datas = new List<GetPageOutput>()
            {
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:01",
                    PartBarCode = "P001",
                    DemQty = 100,
                    RealQty = 101.88m,
                    BatchNum = "B001"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200.89m,
                    RealQty = 201,
                    BatchNum = "B002"
                },
                new GetPageOutput()
                {
                    SendTime = "2023-03-07 17:01:02",
                    PartBarCode = "P002",
                    DemQty = 200.78m,
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
                    DemQty = 200.897m,
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
                    DemQty = 200.8765m,
                    RealQty = 201,
                    BatchNum = "B002"
                },
            };

            return datas;
        } 
    }
}
