using ArsIdentityService4Client;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "Cookies";
    options.DefaultChallengeScheme = "oidc";
})
.AddCookie("Cookies")
.AddOpenIdConnect("oidc", options =>
 {
     options.Authority = "http://localhost:5105";
     options.RequireHttpsMetadata = false;
     options.ClientId = "pcke-key";
     options.ClientSecret = "pcke-secret";
     options.ResponseType = "code";
     options.Scope.Add("ids4-scope"); //添加授权资源
     options.SaveTokens = true;
     options.GetClaimsFromUserInfoEndpoint = true;
 });


builder.Services.ConfigureNonBreakingSameSiteCookies();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
