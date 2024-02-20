using Ars.Common.EFCore.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArsWebApiService.Model.MyCatModel
{
    public class TaskDetail : IEntity<int>
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column("org_code")]
        [StringLength(32)]
        public string? OrgCode { get; set; }

        [Column("plate_no")]
        [StringLength(8)]
        public string? PlateNo { get; set; }

        [Column("task_id")]
        public int TaskId { get; set; }
    }
}
