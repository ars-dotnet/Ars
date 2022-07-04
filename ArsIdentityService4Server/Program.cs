using Ars.Commom.Host.Extension;
using Ars.Common.IdentityServer4.Extension;
using IdentityServer4.Models;
using static Ars.Common.IdentityServer4.options.ArsIdentityServerOption;
using static IdentityServer4.IdentityServerConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var arsbuilder = builder.Services.AddArserviceCore(builder);
arsbuilder.AddArsIdentityServer(
    option => 
    {
        option.ArsClients = option.ArsClients.Concat(
            new List<ArsApiClient>
            {
                new ArsApiClient 
                {
                    AppKey = "admin",
                    AppSecret = "123456",
                    AccessTokenLifetime = 99900,
                    AllowedScopes = new [] { "defaultApi-scope",StandardScopes.OfflineAccess },
                    GrantType = GrantTypes.ClientCredentials
                }
            });
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseArsIdentityServer();

app.MapRazorPages();

app.Run();
