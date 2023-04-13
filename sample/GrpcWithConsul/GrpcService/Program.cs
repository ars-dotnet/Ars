using Ars.Commom.Host.Extension;
using Ars.Commom.Tool.Certificates;
using Ars.Common.Consul.Extension;
using Ars.Common.Consul.IApplicationBuilderExtension;
using Ars.Common.IdentityServer4.Extension;
using Ars.Common.SkyWalking.Extensions;
using GrpcService;
using GrpcService.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services
    .AddArserviceCore(builder.Host,config => 
    {
        config.AddArsIdentityClient();
        config.AddArsConsulRegisterServer();
        config.AddArsSkyApm();
    });
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(kestrel =>
{
    kestrel.Limits.Http2.MaxStreamsPerConnection = 100;
    //kestrel.ConfigureEndpointDefaults(i =>
    //{
    //    i.Protocols = HttpProtocols.Http1AndHttp2;
    //});
    kestrel.Listen(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7903), option =>
    {
        option.Protocols = HttpProtocols.Http1AndHttp2;
        var serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "IS4.pfx");
        option.UseHttps(serverPath, "aabb1212");
    });
});

var app = builder.Build();

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<HealthCheckService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.UseArsCore();

app.Run();
