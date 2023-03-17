using System.Collections;

namespace Ars.Common.Tool.Tools
{
    public class ExportExcelInput
    {
        /// <summary>
        /// 导出excel文件名称
        /// </summary>
        public string ExportFileName { get; set; }

        /// <summary>
        /// 导出列[字段名、显示名]
        /// </summary>
        public IDictionary<string, string> Column { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IEnumerable List { get; set; }

        public Type ItemType { get; set; }
    }
}
