using Ars.Commom.Host.Extension;
using Ars.Commom.Tool.Certificates;
using Ars.Common.Consul.Extension;
using Ars.Common.Consul.IApplicationBuilderExtension;
using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.IdentityServer4.Extension;
using Ars.Common.SkyWalking.Extensions;
using Ars.Common.Tool.Configs;
using GrpcService.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using WebApiGrpcServices.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddTransient<ArsResourcePasswordValidator>();
builder.Services
    .AddArserviceCore(builder, config =>
    {
        config.AddArsIdentityClient();
        config.AddArsConsulRegisterServer();
        config.AddArsSkyApm();
    });
builder.Services.AddGrpc();

//builder.WebHost.UseUrls("https://127.0.0.1:5134");
builder.WebHost.ConfigureKestrel(kestrel => 
{
    kestrel.Limits.Http2.MaxStreamsPerConnection = 100;
    //kestrel.ConfigureHttpsDefaults(i =>
    //{
    //    i.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
    //    i.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls | SslProtocols.None | SslProtocols.Tls11;
    //    i.ClientCertificateValidation = (certificate2, chain, error) => true;
    //    i.ServerCertificate = Certificate.Get("Certificates//IS4.pfx","aabb1212");
    //});
    //kestrel.ConfigureEndpointDefaults(i =>
    //{
    //    i.Protocols = HttpProtocols.Http1AndHttp2;
    //});

    var basicfg = builder.Configuration.GetSection(nameof(ArsBasicConfiguration)).Get<ArsBasicConfiguration>();
    kestrel.Listen(new IPEndPoint(IPAddress.Parse(basicfg.Ip), basicfg.Port), option =>
    {
        option.Protocols = HttpProtocols.Http1AndHttp2;
        var serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "IS4.pfx");
        option.UseHttps(serverPath, "aabb1212");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
//}

app.UsArsExceptionMiddleware();

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

app.UseRouting();
app.UseArsCore();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGrpcService<GreeterService>().EnableGrpcWeb();
    endpoints.MapGrpcService<HealthCheckService>().EnableGrpcWeb();
    endpoints.MapGet("healthCheck", context =>
    {
        return context.Response.WriteAsync("ok");
    });
});


app.Run();
