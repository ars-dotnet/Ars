using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel
{
    public class ArsExcelSaveAllException : ArsExcelException
    {
        public ArsExcelSaveAllException(string message) : base(message)
        {

        }
    }
}
