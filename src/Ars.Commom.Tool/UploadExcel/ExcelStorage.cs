using Ars.Common.Tool.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    internal class ExcelStorage : IExcelStorage
    {
        public Task<bool> ExcelSave(ExcelSaveScheme input)
        {
            return ExcelTool.SaveExcel(input);
        }
    }
}
