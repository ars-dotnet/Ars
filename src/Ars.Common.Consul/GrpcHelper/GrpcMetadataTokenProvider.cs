using Ars.Commom.Tool.Extension;
using Ars.Common.Core.Configs;
using Ars.Common.Core.IDependency;
using Ars.Common.Tool;
using Ars.Common.Tool.Tools;
using Grpc.Core;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ars.Common.Consul.GrpcHelper
{
    internal class GrpcMetadataTokenProvider : IGrpcMetadataTokenProvider, ISingletonDependency
    {
        private readonly IToken _token;
        public GrpcMetadataTokenProvider(
            IToken token)
        {
            _token = token;
        }

        public virtual async Task<Metadata?> GetMetadataToken(ConsulConfiguration option)
        {
            Metadata? entries = null;
            if (option.Communication.UseIdentityServer4Valid)
            {
                entries = new Metadata();
                var token = await _token.GetToken(option);
                entries.Add("Authorization", $"Bearer {token}");
            }

            return entries;
        }
    }
}
