using Ars.Commom.Host.Extension;
using Ars.Common.IdentityServer4.Extension;
using ArsIdentityService4Server;
using IdentityServer4.Models;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services
    .AddArserviceCore(builder.Host,config => config.AddArsIdentityServer());

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
app.UseArsCore();
app.MapDefaultControllerRoute();
app.Run();
