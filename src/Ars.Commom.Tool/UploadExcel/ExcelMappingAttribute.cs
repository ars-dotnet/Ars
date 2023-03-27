using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Tool.UploadExcel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelMappingAttribute : Attribute
    {
        public ExcelMappingAttribute(string column) : this(column, ReadOrWrite.Read)
        {

        }

        public ExcelMappingAttribute(string column, ReadOrWrite readOrWrite)
        {
            Column = column;
            ReadOrWrite = readOrWrite;
        }

        /// <summary>
        /// 对应excel里面的column
        /// </summary>
        public string Column { get; set; }

        public ReadOrWrite ReadOrWrite { get; set; }

        public string Property { get; set; }

        public Type PropertyType { get; set; }
    }
}
