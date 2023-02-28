using Microsoft.AspNetCore.Mvc;

namespace MyApiWithIdentityServer4.Dtos
{
    public class LoginInput
    {
        public string UserName { get; set; }

        public string PassWord { get; set; }
    } 
}
