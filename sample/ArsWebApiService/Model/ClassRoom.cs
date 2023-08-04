using Ars.Common.EFCore.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArsWebApiService.Model
{
    public class ClassRoom : IEntity<Guid>, ICreateEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public int? CreationUserId { get; set; }
        public DateTime? CreationTime { get; set; }
    }
}
