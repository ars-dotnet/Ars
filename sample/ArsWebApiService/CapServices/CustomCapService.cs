using Ars.Common.Cap;
using Ars.Common.Core.IDependency;
using Ars.Common.EFCore.Repository;
using MyApiWithIdentityServer4.Model;

namespace ArsWebApiService.CapServices
{
    [ArsCapSubscribe("ars")]
    public class CustomCapService : IArsCapSubscribe
    {
        private readonly IRepository<Student, Guid> _stuRepo;
        public CustomCapService(IRepository<Student, Guid> stuRepo)
        {
            _stuRepo = stuRepo;
        }

        [ArsCapSubscribe("cap.uowtest",isPartial:true)]
        public async Task SubscribeWithTransaction(string name) 
        {
            var info = await _stuRepo.FirstOrDefaultAsync(r => r.FirstMidName.Equals("aabb1212"));
            info!.LastName = name;

            return;
        }
    }
}
