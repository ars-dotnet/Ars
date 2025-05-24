using Ars.Common.EFCore.Entities;
using System.ComponentModel.DataAnnotations;

namespace ArsWebApiService.Model
{
    public class StudentNew : IEntity<Guid>, IMayHaveTenant,
        ICreateEntity, IModifyEntity, ISoftDelete, IDeleteEntity
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        public int Age { get; set; }

        [Range(1,200)]
        public int Age2 { get; set; }

        public int? TenantId { get; set; }
        public int? CreationUserId { get; set; }
        public DateTime? CreationTime { get; set; }
        public int? UpdateUserId { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
        public int? DeleteUserId { get; set; }
        public DateTime? DeleteTime { get; set; }
    }
}
