using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel
{
    public interface IExcelModel
    {
        /// <summary>
        /// 是否有错
        /// </summary>
        bool IsErr { get; set; }

        /// <summary>
        /// key 错误列
        /// value 具体错误
        /// </summary>
        IDictionary<string, string>? FieldErrMsg { get; set; }
    }
}
