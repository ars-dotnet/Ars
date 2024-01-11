using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel
{
    public abstract class ExcelBaseData<T> : IExcelData<T>
        where T : IExcelModel
    {
        /// <summary>
        /// 文件
        /// </summary>
        [Required]
        public IFormFile File { get; set; }

        /// <summary>
        /// 不填
        /// </summary>
        public IList<T> ExcelModels { get; set; }

        /// <summary>
        /// 数据行从第几行开始
        /// </summary>
        [Required]
        public int ExcelColumnFromRow { get; set; }

        public virtual bool Validation()
        {
            return true;
        }
    }
}
