using Ars.Commom.Host.Extension;
using Ars.Commom.Tool.Certificates;
using Ars.Common.Consul.Extension;
using Ars.Common.Consul.GrpcHelper;
using Ars.Common.Consul.IApplicationBuilderExtension;
using Ars.Common.Core.Extensions;
using Ars.Common.EFCore.Extension;
using Ars.Common.Host.Extension;
using Ars.Common.IdentityServer4.Extension;
using Ars.Common.SkyWalking.Extensions;
using ArsGrpcService.DbContexts;
using ArsGrpcService.Services;
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
    .AddArserviceCore(builder,config => 
    {
        config
            .AddArsIdentityClient()
            .AddArsConsulRegisterServer()
            .AddArsSkyApm()
            .AddArsDbContext<GrpcDbContext>();
    }).AddArsHttpClient();
builder.Services.AddGrpc();

builder.Services.AddSingleton<IEndpointFilter, GrpcUowActionFilter>();

builder.WebHost.UseArsKestrel(builder.Configuration);

var app = builder.Build();

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<HealthCheckService>();
app.MapGrpcService<GrpcDbContextService>()
    .AddEndpointFilter<GrpcServiceEndpointConventionBuilder, GrpcUowActionFilter>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.UseArsCore();

app.Run();
