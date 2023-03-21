using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    public abstract class ExcelBaseData<T> : IExcelData<T>
        where T : IExcelModel
    {
        [Required]
        public IFormFile File { get; set; }

        public IList<T> ExcelModels { get; set; }

        [Required]
        public int ExcelColumnFromRow { get; set; }
    }
}
