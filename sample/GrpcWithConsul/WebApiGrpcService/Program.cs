using Ars.Commom.Host.Extension;
using Ars.Common.Consul.IApplicationBuilderExtension;
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
    .AddArserviceCore(builder.Host)
    .AddArsConsulRegisterServer();
builder.Services.AddGrpc();

builder.WebHost.ConfigureKestrel(kestrel => 
{
    kestrel.Limits.Http2.MaxStreamsPerConnection = 100;
    kestrel.ConfigureHttpsDefaults(i =>
    {
        var serverPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Certificates", "IS4.pfx");
        i.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
        i.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls | SslProtocols.None | SslProtocols.Tls11;
        i.ClientCertificateValidation = (certificate2, chain, error) => true;
        i.ServerCertificate =
            new X509Certificate2(serverPath, "aabb1212");
    });
    kestrel.ConfigureEndpointDefaults(i =>
    {
        //i.Protocols = HttpProtocols.Http2;
    });
    kestrel.Listen(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5134));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseGrpcWeb(new GrpcWebOptions { DefaultEnabled = true });

app.UseHttpsRedirection();

app.UseRouting();
app.UseArsCore();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapGrpcService<GreeterService>().EnableGrpcWeb();
    endpoints.MapGrpcService<HealthCheckService>().EnableGrpcWeb();//.RequireAuthorization();
    endpoints.MapGet("healthCheck", context =>
    {
        return context.Response.WriteAsync("ok");
    });
});

app.Run();
