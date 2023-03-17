using System.Collections;

namespace Ars.Common.Tool.Tools
{
    public class ExcelToolInput
    {
        /// <summary>
        /// 导出excel文件名称
        /// </summary>
        public string ExportFileName { get; set; }

        /// <summary>
        /// 标题名称
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 头信息
        /// </summary>
        public IDictionary<string, (int, int)> Header { get; set; }

        /// <summary>
        /// 导出列[字段名、显示名]
        /// </summary>
        public IDictionary<string, string> Column { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IEnumerable List { get; set; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public Type ItemType { get; set; }
    }
}
