namespace ArsWebApiService.Dtos
{
    public class GetPageInput
    {
        public string PlanNumber { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }

    public class GetPageOutput
    {
        public string SendTime { get; set; }

        public string PartBarCode { get; set; }

        public int DemQty { get; set; }

        public int RealQty { get; set; }

        public string BatchNum { get; set; }
    }
}
