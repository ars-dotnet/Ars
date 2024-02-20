using Ars.Common.EFCore.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArsWebApiService.Model.MyCatModel
{
    public class Product : IEntity<int>
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column("product_name")]
        [StringLength(32)]
        public string ProductName { get; set; }

        [Column("product_code")]
        [StringLength(16)]
        public string ProductCode { get; set; }

        [Column("create_user")]
        public int CreateUser { get; set; }
    }
}
