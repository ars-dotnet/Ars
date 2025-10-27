using Ars.Commom.Host.Extension;
using Ars.Common.Ocelot.Extension;
using Ars.Common.Host.Extension;
using Microsoft.OpenApi.Models;
using Ars.Common.IdentityServer4.Options;
using Ars.Common.IdentityServer4.Extension;
using Ars.Common.Consul.Extension;
using System.Net;
using ArsApiGateway;
using Ars.Common.Ocelot;
using Ocelot.Provider.Consul;
using Ars.Common.Core.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<X509CertificateDelegatingHandler>();

builder.Services.AddArserviceCore(builder,config => 
{
    config.AddArsOcelot(option => 
    {
        //下游https请求颁发证书
        option.AddDelegatingHandler<X509CertificateDelegatingHandler>(true);
    });
});

builder.Services.AddCors(cors =>
{
    cors.AddPolicy("*", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod()
            .WithOrigins("http://127.0.0.1:63042"); //ws跨域配置
    });
});

builder.WebHost.UseArsKestrel(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("*");

app.UseArsCore();

app.UseMiddleware(typeof(ApiGateWayMiddWare));

app.MapControllers();

app.MapGet("/",httpcontext => Task.Run(() => { httpcontext.Response.Redirect("/swagger"); }));

app.Run();
