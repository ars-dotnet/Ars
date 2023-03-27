using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    public interface IExcelData<T> : IExcelDataValidation
        where T : IExcelModel
    {
        IFormFile File { get; set; }

        IList<T> ExcelModels { get; set; }

        /// <summary>
        /// column header开始行
        /// </summary>
        int ExcelColumnFromRow { get; set; }
    }
}
