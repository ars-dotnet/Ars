using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    public interface IExcelResolve
    {
        bool Successed { get; }

        string? ErrorMsg { get; }

        /// <summary>
        /// 是否校验cell失败
        /// </summary>
        bool ValidCellFailed { get; } 

        /// <summary>
        /// 最大行数
        /// </summary>
        int MaxRowCount { get; }

        /// <summary>
        /// column开始行数
        /// </summary>
        int ExcelColumnFromRow { get; set; }

        ExcelResolveResult? ToList<T>(Stream stream)
            where T : IExcelModel, new();
    }
}
