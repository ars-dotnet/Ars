using System.ComponentModel.DataAnnotations;

namespace MyApiWithIdentityServer4.Model
{
    public class Student
    {
        [Key]
        public int ID { get; set; }
        public string LastName { get; set; }
        public string FirstMidName { get; set; }

        public DateTime EnrollmentDate { get; set; }
        
        /// <summary>
        /// 并发标记
        /// </summary>
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
