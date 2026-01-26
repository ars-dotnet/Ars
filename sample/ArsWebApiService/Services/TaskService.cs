using Ars.Commom.Tool.Extension;
using Ars.Common.Core.AspNetCore.OutputDtos;
using Ars.Common.Core.IDependency;
using Ars.Common.EFCore.Repository;
using ArsWebApiService.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyApiWithIdentityServer4.Model;
using NPOI.SS.Formula.Functions;
using System.Linq;

namespace ArsWebApiService.Services
{
    public class TaskService : ITaskService
    {
        [Autowired]
        public IRepository<Student, Guid> Repo { get; set; }

        public async Task<ArsOutput<PageOutput<Student>>> GetList([FromQuery] Search<TestInput> input)
        {
            var count = await Repo.CountAsync();

            var list = await Repo.GetAll()
                .WhereIf(r => r.LastName.Equals(input.Data.PlanNumber),
                              input.Data?.PlanNumber.IsNotNullOrEmpty() ?? false)
                .Skip((input.PageIndex - 1) * input.PageSize)
                .Take(input.PageSize).ToListAsync();

            return new ArsOutput<PageOutput<Student>>(new PageOutput<Student>(count, list));
        }
    }
}
