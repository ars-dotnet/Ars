using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Topro.CombRetLine.HttpApi.Contract.MesXmlModel
{
    [DataContract]
    [XmlRoot("CallAgv"), XmlType("CallAgv")]
    public class CallMesXmlOutput
    {
        /// <summary>
        /// 0 成功
        /// 其它 失败
        /// </summary>
        [DataMember(Name = "<errorCode>k__BackingField"), XmlAttribute]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "<errorMsg>k__BackingField"), XmlAttribute]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "<resultData>k__BackingField"), XmlAttribute]
        public string ResultData { get; set; }
    }
}
