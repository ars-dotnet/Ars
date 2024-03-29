﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel.Validation
{
    public class ExcelStringLengthAttribute : StringLengthAttribute, IExcelValidation
    {
        public ExcelStringLengthAttribute(int maximumLength) : base(maximumLength)
        {

        }

        public string? ErrorMsg { get; protected set; }

        public bool Validation(string filed, object value)
        {
            var check = base.IsValid(value);
            if (!check)
                ErrorMsg = string.Format(ErrorMessageString!, filed, MaximumLength);

            return check;
        }
    }
}
