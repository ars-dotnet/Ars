using Microsoft.AspNetCore.Authentication;

namespace ArsIdentityService4Server.Extendsions
{
    public static class HttpContextExtensions
    {
        public static async Task<bool> GetSchemeSupportsSignOutAsync(this HttpContext context, string scheme)
        {
            var provider = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
            var handler = await provider.GetHandlerAsync(context, scheme);
            return (handler != null && handler is IAuthenticationSignOutHandler);
        }
    }
}
