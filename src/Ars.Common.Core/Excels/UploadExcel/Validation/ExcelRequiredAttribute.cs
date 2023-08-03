using Ars.Commom.Tool.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel.Validation
{
    public class ExcelRequiredAttribute : ExcelBaseValidationAttribute
    {
        public override bool Validation(string filed, object value)
        {
            bool check = value.ToString().IsNotNullOrEmpty();
            if (!check)
                ErrorMsg = "不能空";

            return check;
        }
    }
}
