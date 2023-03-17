using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.Export
{
    public interface IExportManager
    {
        Task<FileStreamResult> GetExcel(ExportExcelInput input);

        IEnumerable? ToEnumerable(object value, Type returnType, string returnEnumerablePropertyName, out Type? itemtype);

        IDictionary<string, string> SetColumn(Type returnType);
    }
}
