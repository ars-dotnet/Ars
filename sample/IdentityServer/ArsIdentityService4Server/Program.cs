using Ars.Commom.Host.Extension;
using Ars.Commom.Tool.Certificates;
using Ars.Common.IdentityServer4.Extension;
using Ars.Common.SkyWalking.Extensions;
using Ars.Common.Tool.Configs;
using Ars.Common.Tool.Extension;
using ArsIdentityService4Server;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services
    .AddArserviceCore(builder.Host, config => 
    {
        config.AddArsIdentityServer();
        config.AddArsSkyApm();
    });

builder.Services.ConfigureNonBreakingSameSiteCookies();

builder.WebHost.ConfigureKestrel(option =>
{
    //容器的ip 192.168.0.7
    //option.Listen(new IPEndPoint(IPAddress.Parse("192.168.0.7"), 5105), listen =>
    //{
    //    listen.Protocols = HttpProtocols.Http1AndHttp2;
    //    listen.UseHttps(Certificate.Get("Certificates//ars.pfx", "aabb1212"));
    //});

    //测试时主机的ip
    //option.Listen(new IPEndPoint(IPAddress.Parse("172.20.64.1"), 5105),listen => 
    //{
    //    listen.Protocols = HttpProtocols.Http1AndHttp2;
    //    listen.UseHttps(Certificate.Get("Certificates//ars.pfx", "aabb1212"));
    //});

    var basicfg = builder.Configuration.GetSection(nameof(ArsBasicConfiguration)).Get<ArsBasicConfiguration>();
    option.Listen(new IPEndPoint(IPAddress.Parse(basicfg.Ip), basicfg.Port), listen =>
    {
        listen.Protocols = HttpProtocols.Http1AndHttp2;
        listen.UseHttps(Certificate.Get("Certificates//ars.pfx", "aabb1212"));
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

app.MapGet("/", context => Task.Run(() => context.Response.Redirect("/.well-known/openid-configuration")));

app.Run();
