using Microsoft.AspNetCore.Mvc;

namespace MyApiWithIdentityServer4.Dtos
{
    public class LoginInput
    {
        public string client_id { get; set; }

        public string client_secret { get; set; }
    }
}
