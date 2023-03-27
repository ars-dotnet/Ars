namespace ArsWebApiService.Dtos
{
    public class GetPageInput : SearchInput
    {
        public string PlanNumber { get; set; }
    }

    public class GetPageOutput
    {
        /// <summary>
        /// 时间
        /// </summary>
        public string SendTime { get; set; }

        /// <summary>
        /// 托盘号
        /// </summary>
        public string PartBarCode { get; set; }

        /// <summary>
        /// 销售计划数量
        /// </summary>
        public decimal DemQty { get; set; }

        /// <summary>
        /// 销售实际数量
        /// </summary>
        public decimal RealQty { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNum { get; set; }
    }

    public class SearchInput 
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class Search<T> : SearchInput
    {
        public T Data { get; set; }
    }

    public class DataInput
    {
        public string Name { get; set; }
    }

    public class DataInputs 
    {
        public string Top { get; set; }
    }
}
