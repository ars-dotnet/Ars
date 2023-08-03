using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel.Validation
{
    public interface IExcelValidation
    {
        string? ErrorMsg { get; }

        bool Validation(string filed, object value);
    }
}
