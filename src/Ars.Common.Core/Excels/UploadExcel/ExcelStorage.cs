﻿using Ars.Common.Tool.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel
{
    internal class ExcelStorage : IExcelStorage
    {
        public Task<bool> SaveExcel(ExcelSaveScheme input)
        {
            return ExcelTool.SaveExcel(input);
        }
    }
}
