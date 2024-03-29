﻿using IdentityModel.Client;

namespace MyApiWithIdentityServer4.Dtos
{
    public class LoginOutput
    {
        public string access_token { get; set; }

        public int expires_in { get; set; }

        public string token_type { get; set; }

        public string refresh_token { get; set; }

        public string scope { get; set; }

        public string user_name { get; set; } 
    }
}
