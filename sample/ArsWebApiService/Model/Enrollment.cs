using Ars.Common.EFCore.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiWithIdentityServer4.Model
{
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment : IEntity<int>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("EnrollmentID")]
        public int Id { get; set; }

        [ForeignKey(nameof(CourseID))]
        public int CourseID { get; set; }

        [ForeignKey(nameof(StudentID))]
        public Guid StudentID { get; set; }

        public Grade? Grade { get; set; }

        public virtual Course Course { get; set; }

        public virtual Student Student { get; set; }
    }
}
