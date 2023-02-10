using Ars.Commom.Host.Extension;
using Ars.Common.Consul.IApplicationBuilderExtension;
using Ars.Common.IdentityServer4.Extension;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMvc();

builder.Services
    .AddArserviceCore(builder.Host);
builder.Services.AddHttpClient();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.Authority = "http://127.0.0.1:5134";
    options.RequireHttpsMetadata = false;
    options.ClientId = "pcke-key";
    options.ClientSecret = "pcke_secret";
    options.SaveTokens = true;
    options.ResponseType = "code";
    options.SkipUnrecognizedRequests = true;
    //options.CallbackPath = "/signin-oidc";
    options.Scope.Clear();
    options.Scope.Add("grpcapi-scope");
    options.Scope.Add(OidcConstants.StandardScopes.OpenId);
    options.Scope.Add(OidcConstants.StandardScopes.Profile);
    options.Scope.Add(OidcConstants.StandardScopes.Email);
    options.Scope.Add(OidcConstants.StandardScopes.Address);
    options.Scope.Add(OidcConstants.StandardScopes.Phone);
    options.Scope.Add(OidcConstants.StandardScopes.OfflineAccess);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseArsCore();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
