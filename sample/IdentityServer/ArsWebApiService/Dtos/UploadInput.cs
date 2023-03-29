using Ars.Common.Tool.UploadExcel;
using Ars.Common.Tool.UploadExcel.Validation;

namespace ArsWebApiService.Dtos
{
    public class UploadInput : ExcelBaseData<UploadModel>
    {
        public override bool Validation()
        {
            return true;
        }
    }

    public class UploadModel : ExcelBaseModel
    {
        [ExcelMapping("时间",true)]
        [ExcelRequired]
        public DateTime Time { get; set; }

        [ExcelMapping("托盘号")]
        [ExcelStringLength(10)]
        public string Barcode { get; set; }

        [ExcelMapping("销售计划数量")]
        [ExcelNumber]
        public decimal Qty { get; set; }

        [ExcelMapping("销售实际数量")]
        [ExcelNumber]
        public decimal ActullyQty { get; set; }

        [ExcelMapping("批次号")]
        [ExcelStringLength(10)]
        public string Batchcode { get; set; }
    }
}
