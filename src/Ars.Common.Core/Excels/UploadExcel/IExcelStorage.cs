﻿using Ars.Common.Tool.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel
{
    public interface IExcelStorage
    {
        Task<bool> SaveExcel(ExcelSaveScheme input);
    }
}
