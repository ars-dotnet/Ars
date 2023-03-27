using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    public class ArsExcelException : ArsException
    {
        public string ErrExcelDownUrl { get; set; }

        public ArsExcelException() : base()
        {

        }

        public ArsExcelException(string message) : this(string.Empty,message)
        {

        }


        public ArsExcelException(string errExcelDownUrl,string message) : base(message)
        {
            ErrExcelDownUrl = errExcelDownUrl;
        }
    }
}
