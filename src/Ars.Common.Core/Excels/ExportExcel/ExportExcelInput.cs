using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.ExportExcel
{
    public class ExportExcelInput
    {
        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public IDictionary<string, object> Params { get; set; }

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
        /// 如果查询结果不是ienumerable类型，且集合数据是结果实体的一个属性，则可标记集合对象的属性名
        /// 如果不写，则默认取查询结果中的第一个集合属性，如果没有集合属性，则将查询结果导出
        /// </summary>
        public string ReturnEnumerablePropertyName { get; set; }
    }
}
