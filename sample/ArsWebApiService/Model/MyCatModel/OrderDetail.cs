using Ars.Common.EFCore.Entities;
using Google.Protobuf.WellKnownTypes;
using MathNet.Numerics;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArsWebApiService.Model.MyCatModel
{
    public class OrderDetail : IEntity<int>
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("pay_amount",TypeName = "decimal(10,2)")]
        public decimal PayAmount { get; set; }

        [Column("create_time")]
        public string CreateTime { get; set; }
    }

    public class OrderDetailQuery : IEntity<int>
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }

        [Column("pay_amount", TypeName = "decimal(10,2)")]
        public decimal PayAmount { get; set; }

        [Column("create_time")]
        public DateTime CreateTime { get; set; }
    }
}
