using System.ComponentModel.DataAnnotations.Schema;

namespace MyApiWithIdentityServer4.Model
{
    public class Course
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int CourseID { get; set; }
        public string Title { get; set; }
        public int Credits { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
