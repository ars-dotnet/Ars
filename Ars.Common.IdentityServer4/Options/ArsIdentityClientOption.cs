using Ars.Common.Core.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.IdentityServer4.Options
{
    internal class ArsIdentityClientOption : IArsIdentityClientConfiguration
    {
        public string Authority { get; set; }

        public string ApiName { get; set; }
    }
}
