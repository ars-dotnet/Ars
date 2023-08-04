namespace ArsWebApiService.Dtos
{
    public class GetPlanByNumberOutput
    {
        /// <summary>
        /// PlanNumber1
        /// </summary>
        public string PlanNumber { get; set; }

        /// <summary>
        /// OrderNum1
        /// </summary>
        public string OrderNum { get; set; }

        /// <summary>
        /// SnNum1
        /// </summary>
        public string SnNum { get; set; }

        /// <summary>
        /// CusName1
        /// </summary>
        public string CusName { get; set; }

        /// <summary>
        /// ProductName
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// BarCode1
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// BomNumber
        /// </summary>
        public string BomNumber { get; set; }

        /// <summary>
        /// PlanQty
        /// </summary>
        public decimal PlanQty { get; set; }
    }
}
