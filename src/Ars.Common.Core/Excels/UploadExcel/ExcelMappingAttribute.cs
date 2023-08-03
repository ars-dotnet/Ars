using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.UploadExcel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcelMappingAttribute : Attribute
    {
        public ExcelMappingAttribute(string column) : this(column, false, ReadOrWrite.Read)
        {

        }

        public ExcelMappingAttribute(string column, bool isRequired) : this(column, isRequired, ReadOrWrite.Read)
        {

        }

        public ExcelMappingAttribute(string column, ReadOrWrite readOrWrite) : this(column, false, readOrWrite)
        {

        }

        public ExcelMappingAttribute(string column, bool isRequired, ReadOrWrite readOrWrite)
        {
            Column = column;
            IsRequired = isRequired;
            ReadOrWrite = readOrWrite;
        }

        /// <summary>
        /// 对应excel里面的column
        /// </summary>
        public string Column { get; set; }

        public ReadOrWrite ReadOrWrite { get; set; }

        public bool IsRequired { get; set; }

        public string Property { get; set; }

        public Type PropertyType { get; set; }
    }
}
