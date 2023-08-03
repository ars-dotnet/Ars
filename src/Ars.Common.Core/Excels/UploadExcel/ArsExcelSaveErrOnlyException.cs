using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel
{
    public class ArsExcelSaveErrOnlyException : ArsExcelException
    {
        public ArsExcelSaveErrOnlyException(string message) : base(message)
        {

        }
    }
}
