using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Topro.CombRetLine.HttpApi.Contract.MesXmlModel
{
    [DataContract()]
    public class CallMesXmlOutput
    {
        /// <summary>
        /// 0 成功
        /// 其它 失败
        /// </summary>
        [DataMember(Name = "<errorCode>k__BackingField")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "<errorMsg>k__BackingField")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "<resultData>k__BackingField")]
        public string ResultData { get; set; }
    }
}
