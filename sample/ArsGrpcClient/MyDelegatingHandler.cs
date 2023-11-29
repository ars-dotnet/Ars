using Newtonsoft.Json;

namespace GrpcClients
{
    public class MyDelegatingHandler : DelegatingHandler
    {
        public MyDelegatingHandler()
        {

        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var res = await base.SendAsync(request, cancellationToken);

            res.EnsureSuccessStatusCode();
            var a = await res.Content.ReadAsStringAsync();

            var m = JsonConvert.DeserializeObject<object>(a);

            return res;
        }
    }
}
