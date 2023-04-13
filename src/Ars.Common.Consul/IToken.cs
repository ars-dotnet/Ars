using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul
{
    public interface IToken
    {
        Task<string> GetToken(ConsulConfiguration consulConfiguration);
    }
}
