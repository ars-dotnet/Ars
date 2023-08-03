using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Core.Configs
{
    public interface IConsulRegisterConfiguration : IConsulConfiguration,IArsCertificateConfiguration, IArsIpPortConfiguration
    {
        string HttpHealthAction { get; }
    }
}
