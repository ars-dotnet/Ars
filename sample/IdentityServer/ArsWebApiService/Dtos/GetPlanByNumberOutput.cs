namespace ArsWebApiService.Dtos
{
    public class GetPlanByNumberOutput
    {
        public string PlanNumber { get; set; }

        public string OrderNum { get; set; }

        public string SnNum { get; set; }

        public string CusName { get; set; }

        public string ProductName { get; set; }

        public string BarCode { get; set; }

        public string BomNumber { get; set; }

        public decimal PlanQty { get; set; }
    }
}
