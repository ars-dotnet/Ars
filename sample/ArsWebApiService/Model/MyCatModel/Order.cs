using Ars.Common.EFCore.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArsWebApiService.Model.MyCatModel
{
    public class Order : IEntity<int>
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column("order_code")]
        [StringLength(32)]
        public string OrderCode { get; set; }

        [Column("create_time")]
        public string CreateTime { get; set; }

        [Column("create_user")]
        public int CreateUser { get; set; }
    }

    public class OrderQuery : IEntity<int>
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column("order_code")]
        [StringLength(32)]
        public string OrderCode { get; set; }

        [Column("create_time")]
        public DateTime CreateTime { get; set; }

        [Column("create_user")]
        public int CreateUser { get; set; }
    }
}
