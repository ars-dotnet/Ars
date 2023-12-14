using Ars.Commom.Host.Extension;
using Ars.Commom.Tool.Certificates;
using Ars.Common.Host.Extension;
using Ars.Common.IdentityServer4.Extension;
using Ars.Common.SkyWalking.Extensions;
using ArsIdentityService4Server;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services
    .AddArserviceCore(builder, config => 
    {
        config
            .AddArsIdentityServer(
                //provider =>
                //{
                //    return provider.GetRequiredService<MyDefaultResourceOwnerPasswordValidator>();
                //}
            )
            .AddArsSkyApm();
    });

builder.Services.AddTransient<MyDefaultResourceOwnerPasswordValidator>();
builder.Services.ConfigureNonBreakingSameSiteCookies();

builder.WebHost.UseArsKestrel(builder.Configuration);

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
