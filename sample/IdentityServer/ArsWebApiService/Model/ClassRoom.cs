using Ars.Common.EFCore.Entities;

namespace ArsWebApiService.Model
{
    public class ClassRoom : IEntity<int>
    {
        public int Id { get; set; }
    }
}
