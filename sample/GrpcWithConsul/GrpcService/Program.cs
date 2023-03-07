using Ars.Commom.Host.Extension;
using Ars.Common.Consul.Extension;
using Ars.Common.Consul.IApplicationBuilderExtension;
using Ars.Common.IdentityServer4.Extension;
using GrpcService;
using GrpcService.Services;
using Microsoft.Extensions.Options;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services
    .AddArserviceCore(builder.Host,config => 
    {
        config.AddArsConsulRegisterServer();
        config.AddArsIdentityServer();
    });
builder.Services.AddGrpc();

builder.WebHost.UseKestrel(kestrel =>
{
    kestrel.Listen(IPAddress.Parse("127.0.0.1"), 7903);
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<HealthCheckService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.UseArsCore();

app.Run();
