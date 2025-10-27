using Ars.Commom.Host.Extension;
using Ars.Commom.Tool.Certificates;
using Ars.Common.Consul.Extension;
using Ars.Common.Consul.IApplicationBuilderExtension;
using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.Core.AspNetCore.Swagger;
using Ars.Common.Core.Extensions;
using Ars.Common.Host.Extension;
using Ars.Common.IdentityServer4.Extension;
using Ars.Common.SkyWalking.Extensions;
using GrpcService.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddArsSwaggerGen(option => 
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "ArsGrpcWebApiService", Version = "v1" });

    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ArsWebApiGrpcService.xml");
    if (File.Exists(path))
    {
        option.IncludeXmlComments(path);
    }

    //枚举显示为字符串
    option.SchemaFilter<EnumSchemaFilter>();
    //根据AuthorizeAttributea分配是否需要授权操作
    option.OperationFilter<SecurityRequirementsOperationFilter>();
});

//builder.Services.AddTransient<ArsResourcePasswordValidator>();
builder.Services
    .AddArserviceCore(builder, config =>
    {
        config
            .AddArsIdentityClient()
            .AddArsConsulRegisterServer()
            .AddArsSkyApm();
    })
    .AddArsHttpClient();
builder.Services.AddGrpc();

//builder.WebHost.UseUrls("https://127.0.0.1:5134");
builder.WebHost.UseArsKestrel(builder.Configuration);

var app = builder.Build();

app.UsArsExceptionMiddleware();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{ 
    app.UseSwagger(option => option.RouteTemplate = "Api/ArsGrpcWebApi/swagger/{documentName}/swagger.json");
    app.UseSwaggerUI(option => option.SwaggerEndpoint("/Api/ArsGrpcWebApi/swagger/v1/swagger.json", "ArsGrpcWebApiService - v1"));
    app.UseDeveloperExceptionPage();
//}

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
