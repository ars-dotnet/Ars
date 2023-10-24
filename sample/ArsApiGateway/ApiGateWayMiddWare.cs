using Ocelot.Middleware;

namespace ArsApiGateway
{
    public class ApiGateWayMiddWare
    {
        private readonly RequestDelegate _next;

        public ApiGateWayMiddWare(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext) 
        {
            var a = httpContext.Items.DownstreamRoute();
            return _next(httpContext);
        }
    }
}
