using Ars.Common.EFCore.Entities;
using MyApiWithIdentityServer4.Model;
using System.ComponentModel.DataAnnotations;

namespace ArsWebApiService.Model
{
    public class StudentMsSql : IEntity<Guid>, IMayHaveTenant,
        ICreateEntity, IModifyEntity, ISoftDelete, IDeleteEntity
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(50)]
        public string FirstMidName { get; set; }

        public DateTime EnrollmentDate { get; set; }

        /// <summary>
        /// 并发标记
        /// </summary>
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }

        //public IEnumerable<JsonProperty> JsonProperty { get; set; }

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
