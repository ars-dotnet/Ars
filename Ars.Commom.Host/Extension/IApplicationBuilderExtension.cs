using Ars.Common.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ars.Commom.Host.Extension
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseArsCore(this IApplicationBuilder applicationBuilder, 
            Action<IArsConfiguration> configuAction = null) 
        {
            if (null != configuAction)
            {
                var arsConfigure = applicationBuilder.ApplicationServices.GetRequiredService<IArsConfiguration>();
                configuAction.Invoke(arsConfigure);
            }

            return applicationBuilder;
        }
    }
}
