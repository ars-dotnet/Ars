using Microsoft.AspNetCore.Builder;

namespace Ars.Commom.Host.Extension
{
    public static class IApplicationBuilderExtension
    {
        public static IApplicationBuilder UseArsCore(this IApplicationBuilder applicationBuilder) 
        {
            
            return applicationBuilder;
        }
    }
}
