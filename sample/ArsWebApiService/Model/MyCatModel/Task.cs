using Ars.Common.EFCore.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArsWebApiService.Model.MyCatModel
{
    public class Task : IEntity<int>
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column("task_code")]
        [StringLength(32)]
        public string TaskCode { get; set; }

        [Column("org_code")]
        [StringLength(32)]
        public string OrgCode { get; set; }

        [Column("create_user")]
        public int CreateUser { get; set; }
    }
}
