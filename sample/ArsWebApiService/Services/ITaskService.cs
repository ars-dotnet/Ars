using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.Excels.ExportExcel;
using Ars.Common.Core.IDependency;
using ArsWebApiService.Dtos;
using Microsoft.AspNetCore.Mvc;
using MyApiWithIdentityServer4.Model;

namespace ArsWebApiService.Services
{
    [ExportService]
    public interface ITaskService : IScopedDependency
    {
        [ExportAction]
        Task<ArsOutput<PageOutput<Student>>> GetList([FromQuery] Search<TestInput> input);
    }
}
