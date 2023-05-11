using Ars.Commom.Host.Extension;
using Ars.Commom.Tool.Certificates;
using Ars.Common.IdentityServer4.Extension;
using Ars.Common.Tool.Extension;
using ArsIdentityService4Server;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services
    .AddArserviceCore(builder.Host,config => config.AddArsIdentityServer());

builder.Services.ConfigureNonBreakingSameSiteCookies();

builder.WebHost.ConfigureKestrel(option =>
{
    option.Listen(new IPEndPoint(IPAddress.Parse("172.20.64.1"), 5105),listen => 
    {
        listen.Protocols = HttpProtocols.Http1AndHttp2;
        listen.UseHttps(Certificate.Get("Certificates\\ars.pfx", "aabb1212"));
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
app.UseCookiePolicy();
app.UseArsCore();
app.MapDefaultControllerRoute();
app.Run();
