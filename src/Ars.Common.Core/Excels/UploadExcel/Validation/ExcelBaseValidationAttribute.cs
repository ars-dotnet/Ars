using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class ExcelBaseValidationAttribute : Attribute, IExcelValidation
    {
        public string? ErrorMsg { get; protected set; }

        public abstract bool Validation(string filed, object value);
    }
}
