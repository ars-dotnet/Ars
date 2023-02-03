using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IConsulConfiguration 
    {
        string ServiceName { get; }

        string ConsulAddress { get; }
    }

    public class ConsulConfiguration 
    {
        public string ServiceName { get; set; }

         public string ConsulAddress { get; set; }

         /// <summary>
         /// 是否用http1通讯
         /// </summary>
         public bool UseHttp1Protocol { get; set; }
    }

    public interface IConsulDiscoverConfiguration
    {
        IEnumerable<ConsulConfiguration> ConsulDiscovers { get; }
    }
}
