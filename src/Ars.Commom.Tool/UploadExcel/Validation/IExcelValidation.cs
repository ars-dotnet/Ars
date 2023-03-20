using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel.Validation
{
    public interface IExcelValidation
    {
        string? ErrorMsg { get; }

        bool Validation(object value);
    }
}
