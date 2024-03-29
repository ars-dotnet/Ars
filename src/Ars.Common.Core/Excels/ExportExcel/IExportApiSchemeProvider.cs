﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.ExportExcel
{
    public interface IExportApiSchemeProvider
    {
        ExportApiScheme? GetExportApiScheme(string key);

        void SetExportApiSchemed(Assembly assembly);
    }
}
