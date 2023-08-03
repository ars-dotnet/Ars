using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel.Validation
{
    public class ExcelRangeAttribute : RangeAttribute, IExcelValidation
    {
        public ExcelRangeAttribute(double minimum, double maximum)
            : base(minimum, maximum)
        {

        }

        public ExcelRangeAttribute(int minimum, int maximum)
            : base(minimum, maximum)
        {

        }

        public ExcelRangeAttribute([DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] Type type, string minimum, string maximum)
            : base(type, minimum, maximum)
        {

        }

        public string? ErrorMsg { get; protected set; }

        public bool Validation(string filed, object value)
        {
            bool check = base.IsValid(value);
            if (!check)
                ErrorMsg = string.Format(ErrorMessageString!, filed, Minimum, Maximum);

            return check;
        }
    }
}
