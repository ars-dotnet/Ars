using IdentityModel.Client;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class AspNetCoreTest
    {
        [Fact]
        public void CreateHost() 
        {
            int[] array = { 1, 2, 3, 4, 5, 6 };
            Array.ForEach(array, r => 
            {
                r++;
            });
            
            
        }

        [Fact]
        public async Task TestIdentityServer() 
        {
            using HttpClient client = new HttpClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:7207");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }
            var tokenresponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                ClientId = "default-Key",
                ClientSecret = "default-Secret",
                Scope = "defaultApi-scope",
                UserName = "test",
                Password = "test",
                Address = "http://localhost:7207/connect/token"
            });
            if(tokenresponse.IsError)
            {
                Console.WriteLine(tokenresponse.Error);
                return;
            }

            var token = tokenresponse.AccessToken;

            //http://localhost:5196/WeatherForecast
            // call api
            var apiClient = new HttpClient();
            apiClient.SetBearerToken(token);

            var response = await apiClient.GetAsync("http://localhost:5196/WeatherForecast");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }

    }
}
