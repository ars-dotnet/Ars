using Ars.Commom.Host.Extension;
using Ars.Common.Consul.Extension;
using Ars.Common.Consul.IApplicationBuilderExtension;
using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.IdentityServer4.Extension;
using Ars.Common.Tool.Extension;
using GrpcClient.ApmLogger;
using GrpcClients;
using Microsoft.Extensions.DependencyInjection.Extensions;

Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "SkyAPM.Agent.AspNetCore");
Environment.SetEnvironmentVariable("SKYWALKING__SERVICENAME", "grpclient");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddArserviceCore(builder, config =>
    {
        config.AddArsIdentityClient();
        config.AddArsConsulDiscoverClient();
        //config.AddArsSkyApm();
    });

builder.Services.AddScoped<IChannelManager, ChannelManager>();
builder.Services.AddSingleton<IChannelProvider, ChannelProvider>();

//builder.Services.AddSingleton<ChannelService>();
//builder.Services.AddHostedService(pro => pro.GetRequiredService<ChannelService>());

//builder.Services.AddSingleton<ChannelPublish>();
//builder.Services.AddHostedService(pro => pro.GetRequiredService<ChannelPublish>());

//builder.Services.AddSingleton<ChannelPublish2>();
//builder.Services.AddHostedService(pro => pro.GetRequiredService<ChannelPublish2>());

builder.Services.AddSkyAPM();

builder.Services.Replace(ServiceDescriptor.Singleton<SkyApm.Logging.ILoggerFactory, MyLoggerFactory>());

var app = builder.Build();

app.UsArsExceptionMiddleware();

// Configure the HTTP request pipeline.
//if (app.Environment.IsProduction())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseArsCore();

app.MapControllers();

app.MapGet("/", context => Task.Run(() => context.Response.Redirect("/swagger")));

app.Run();
