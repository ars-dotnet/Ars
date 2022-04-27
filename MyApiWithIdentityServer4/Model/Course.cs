using Ars.Common.EFCore.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiWithIdentityServer4.Model
{
    public class Course : ISoftDelete, IMayHaveTenant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }
        public string Title { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Credits { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }

        public bool IsDeleted { get; set; }
        public int? TenantId { get; set; }
    }
}
