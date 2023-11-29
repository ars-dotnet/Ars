using Ars.Common.Core.IDependency;
using Autofac.Core;
using Microsoft.EntityFrameworkCore;

namespace ArsWebApiService.Services
{
    public class Service : IService,ITransientDependency
    {
        private readonly MyDbContext _myDbContext;
        private readonly IService1 _service1;
        public Service(MyDbContext myDbContext, IService1 service1)
        {
            _myDbContext = myDbContext; 
            _service1 = service1;
        }

        public async Task Get()
        {
            var a = _myDbContext.GetHashCode();

            await _service1.Get();

            //IList<Task> tasks = new List<Task>()
            //{
            //    _myDbContext.Students.FirstOrDefaultAsync(),

            //    _service1.Get(),

            //    _myDbContext.Students.FirstOrDefaultAsync(),

            //    _myDbContext.Students.FirstOrDefaultAsync(),

            //    _myDbContext.Students.FirstOrDefaultAsync(),
            //};

            //await Task.WhenAll(tasks);
        }
    }

    public interface IService1 
    {
        Task Get();
    }

    public class Service1 : IService1, ITransientDependency
    {
        private readonly MyDbContext _myDbContext;

        public Service1(MyDbContext myDbContext)
        {
            _myDbContext = myDbContext;
        }

        public async Task Get()
        {
            var a = _myDbContext.GetHashCode();

            var data = await _myDbContext.Students.FirstOrDefaultAsync();
        }
    }
}
