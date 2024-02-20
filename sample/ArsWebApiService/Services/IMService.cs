using Ars.Common.Core.IDependency;
using Ars.Common.EFCore.Repository;
using Microsoft.EntityFrameworkCore;
using MyApiWithIdentityServer4.Model;

namespace ArsWebApiService.Services
{
    public interface IMService : IScopedDependency
    {
        Task<Student> GetAsync();
    }

    public class MService : IMService 
    {
        private readonly IRepository<Student,Guid> _repo;
        public MService(IRepository<Student, Guid> repo)
        {
            _repo = repo;
        }

        public async Task<Student> GetAsync() 
        {
            var m = await _repo.GetAll().FirstOrDefaultAsync();

            return m;
        }
    }
}
