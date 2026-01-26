namespace ArsWebApiService.Dtos
{
    public class PageOutput<T>
    {
        /// <summary>
        /// 总条目数
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        public IEnumerable<T>? list { get; set; }

        public PageOutput(int total, IEnumerable<T>? list)
        {
            this.total = total;
            this.list = list;
        }
    }
}
