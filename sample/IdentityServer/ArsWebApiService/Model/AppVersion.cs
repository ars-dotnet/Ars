using Ars.Common.EFCore.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArsWebApiService.Model
{
    public class AppVersion : IEntity<int>, ICreateEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(10)]
        public string Version { get; set; }

        [StringLength(50)]
        public string Path { get; set; }

        public int? CreationUserId { get; set; }

        public DateTime? CreationTime { get; set; }
    }
}
