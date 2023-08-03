using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Excels.ExportExcel
{
    public interface IXmlFileManager
    {
        /// <summary>
        /// 获取当前类中属性的summary
        /// 如果没有，就返回propertyName的值
        /// </summary>
        /// <param name="classType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        string GetPropertyXmlSummary(Type classType, string propertyName);

        void SetXmlResource(string xmlpath);
    }
}
