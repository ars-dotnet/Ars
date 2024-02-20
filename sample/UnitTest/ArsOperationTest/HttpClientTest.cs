using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsOperationTest
{
    public class HttpClientTest
    {
        /// <summary>
        /// 测试get请求参数放body
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task TestGetWithXwwwFormUrlencoded()
        {
            using HttpClient httpClient = new HttpClient();

            using var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost:5196/Api/ArsWebApi/V1.0/Excel/UplaodByGet"),
                Content = new StringContent("PlanNumber='123'&PageIndex=1&PageSize10", Encoding.UTF8, "application/x-www-form-urlencoded"),
            };

            using var res = await httpClient.SendAsync(request);

            res.EnsureSuccessStatusCode();

            var str = await res.Content.ReadAsStringAsync();
        }
    }
}
