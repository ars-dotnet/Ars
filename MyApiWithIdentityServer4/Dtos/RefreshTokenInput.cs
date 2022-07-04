namespace MyApiWithIdentityServer4.Dtos
{
    public class RefreshTokenInput : LoginInput
    {
        public string refresh_token { get; set; }
    }
}
