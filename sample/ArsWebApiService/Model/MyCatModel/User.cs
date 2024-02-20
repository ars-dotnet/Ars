using Ars.Common.EFCore.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArsWebApiService.Model.MyCatModel
{
    /// <summary>
    /// 
    /// </summary>
    public class User : IEntity<int>
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column("name")]
        [StringLength(8)]
        public string Name { get; set; }

        [Column("age")]
        public int Age { get; set; }

        [Column("create_time")]
        public DateTime CreateTime { get; set; }

        [Column("section_code")]
        [StringLength(8)]
        public string SectionCode { get; set; }
    }
}
