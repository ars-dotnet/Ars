using Ars.Commom.Host.Extension;
using Ars.Common.IdentityServer4.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApiWithIdentityServer4;
using Ars.Common.EFCore.Extension;
using Ars.Common.Core.AspNetCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Ars.Common.Core.Configs;
using Ars.Common.Core.AspNetCore.Swagger;
using Ars.Common.IdentityServer4.Validation;
using System.Net;
using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.Tool.Extension;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using Ars.Common.IdentityServer4.Options;
using Ars.Common.Redis.Extension;
using Ars.Common.SignalR.Extensions;
using Ars.Common.SignalR.Hubs;
using System.Security.Authentication;
using Ars.Commom.Tool.Certificates;
using IdentityModel.AspNetCore.OAuth2Introspection;
using ArsWebApiService.Hubs;
using Ars.Common.SignalR.Sender;
using Ars.Common.SkyWalking.Extensions;
using Ars.Common.Consul.Extension;
using Autofac.Core;
using ArsWebApiService.WebServices;
using SoapCore;
using System.ServiceModel;
using Ars.Common.Host.Extension;
using Ars.Common.Cap.Extensions;
using Ars.Common.Core.Localization.Extension;
using Ars.Common.Core.Extensions;
using static IdentityModel.ClaimComparer;
using ArsWebApiService;
using Ars.Common.EFCore;
using Com.Ctrip.Framework.Apollo;
using Com.Ctrip.Framework.Apollo.Enums;

var builder = WebApplication.CreateBuilder(args);

// add apollo service
builder.WebHost.ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
{
    configurationBuilder
        .AddApollo(hostBuilderContext.Configuration.GetSection("apollo"))
        .AddDefault();
});

// Add services to the container.
var arsbuilder =
    builder.Services
    .AddArserviceCore(builder, config =>
    {
        config
            .AddArsIdentityClient()
            .AddArsRedis(provider =>
            {
                provider.ConfigureAll(cacheoption =>
                {
                    cacheoption.DefaultSlidingExpireTime = TimeSpan.FromMinutes(10);
                });
            })
            .AddArsSignalR(arsConfig =>
            {
                arsConfig.CacheType = 0;
                arsConfig.UseMessagePackProtocol = true;
            },hubConfig => 
            {
                hubConfig.KeepAliveInterval = TimeSpan.FromSeconds(30);
                hubConfig.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
            })
            .AddArsConsulRegisterServer()
            .AddArsSkyApm()
            .AddArsCap(option =>
            {
                option.UseEntityFramework<MyDbContext>();

                option.UseRabbitMQ(mq =>
                {
                    mq.HostName = "localhost";
                    mq.UserName = "guest";
                    mq.Password = "guest";
                });
            })
            .AddArsMultipleDbContext<MyDbContext>()
            .AddArsMultipleDbContext<MyDbContext2>()
            .AddArsMultipleDbContext<MyCatDbContext>()
            .AddArsMultipleDbContext<MyCatQueryDbContext>()
            .AddArsMultipleDbContext<MyDbContextWithMsSql>();
    })
    //.AddArsDbContext<MyDbContext>()
    .AddArsHttpClient()
    .AddArsExportExcelService(typeof(Program).Assembly)
    .AddArsUploadExcelService(option =>
    {
        option.UploadRoot = "wwwroot/upload";
        option.RequestPath = "apps/upload";
        option.SlidingExpireTime = TimeSpan.FromDays(1);
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var mm =  builder.Services.Where(r => r.ServiceType == typeof(MyDbContext)).ToList();

builder.Services.AddCors(cors =>
{
    cors.AddPolicy("*", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod()
            .WithOrigins("http://127.0.0.1:63042");
    });
});

builder.Services.AddArsSwaggerGen(
    builder.Configuration.
    GetSection(nameof(ArsIdentityClientConfiguration)).
    Get<ArsIdentityClientConfiguration>());

builder.WebHost.UseArsKestrel(builder.Configuration);

//builder.Services.AddDbContext<MyDbContext>();

builder.Services.AddScoped<IHubSendMessage, MyWebHub>();
builder.Services.AddScoped<IWebServices, WebServices>();

//添加版本控制
builder.Services.AddApiVersioning().AddApiExplorer();

// Configure the HTTP request pipeline.
var app = builder.Build();

app.UsArsExceptionMiddleware();

app.UseSwagger(option =>
{
    option.RouteTemplate = "Api/ArsWebApi/swagger/{documentName}/swagger.json";
});
//app.UseSwaggerUI(option =>
//{
//    option.SwaggerEndpoint("/Api/ArsWebApi/swagger/v1/swagger.json", "ArsWebApiService - v1"); //这里的v1表示文档名称documentName
//}); 
app.UseArsSwaggerUI();

app.UseCors("*");

//上传app文件
string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "AppDownload");
if (!Directory.Exists(path))
    Directory.CreateDirectory(path);
app.UseDirectoryBrowser(new DirectoryBrowserOptions
{
    FileProvider = new PhysicalFileProvider(path),
    RequestPath = "/apps/download"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(path),
    RequestPath = "/apps/download",
    ContentTypeProvider = new FileExtensionContentTypeProvider(
        new Dictionary<string, string>
        {
            { ".apk","application/vnd.android.package-archive"},
        })
});

app.UseArsCore().UseArsUploadExcel();

app.MapControllers();
app.MapHub<MyWebHub>("/ws/webapi/web/hub");
app.MapHub<ArsAndroidHub>("/ws/webapi/android/hub");

app.MapGet("/", context => Task.Run(() => context.Response.Redirect("/swagger")));
app.Map("/healthCheck", builder => builder.Run(context => context.Response.WriteAsync("ok")));

//app.UseSoapEndpoint<IWebServices>("/StudentService.asmx", new BasicHttpBinding(), SoapSerializer.XmlSerializer);
((IApplicationBuilder)app).UseSoapEndpoint<IWebServices>("/WebServices.asmx", new SoapEncoderOptions(), serializer: SoapSerializer.XmlSerializer);

app.Run();
