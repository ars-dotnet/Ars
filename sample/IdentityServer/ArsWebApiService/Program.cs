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
using Ars.Common.Tool.Swagger;
using Ars.Common.IdentityServer4.Validation;
using System.Net;
using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.Tool.Extension;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using Ars.Common.Consul.Extension;
using Ars.Common.SkyWalking.Extensions;
using Ars.Common.Tool.Configs;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

var arsbuilder =
    builder.Services
    .AddArserviceCore(builder.Host, config =>
    {
        config.AddArsIdentityClient();
        config.AddArsConsulRegisterServer();
        config.AddArsSkyApm();
    })
    .AddArsDbContext<MyDbContext>();
builder.Services
    .AddArsHttpClient()
    .AddArsExportExcelService(typeof(Program).Assembly)
    .AddArsUploadExcelService(option => 
    {
        option.UploadRoot = "wwwroot/upload";
        option.RequestPath = "apps/upload";
        option.SlidingExpireTime = TimeSpan.FromMinutes(30);
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(cors =>
{
    cors.AddPolicy("*", policy =>
    {
        policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
    });
});

using var scope = builder.Services.BuildServiceProvider().CreateScope();
var idscfg = scope.ServiceProvider.GetRequiredService<IArsIdentityClientConfiguration>();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ArsWebApiService", Version = "v1" });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Password = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{idscfg.Authority}/connect/authorize", UriKind.Absolute),
                TokenUrl = new Uri($"{idscfg.Authority}/connect/token", UriKind.Absolute),
                Scopes = new Dictionary<string, string>()
                {
                    { "grpcapi-scope","授权读写操作" }
                }
            }
        }
    });

    //Influencer.GraphqlAggregator.xml
    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ArsWebApiService.xml");
    if (File.Exists(path))
    {
        c.IncludeXmlComments(path);
    }

    //枚举显示为字符串
    c.SchemaFilter<EnumSchemaFilter>();
    //根据AuthorizeAttributea分配是否需要授权操作
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

var basiccfg = scope.ServiceProvider.GetRequiredService<IOptions<IArsBasicConfiguration>>().Value;
builder.WebHost.UseKestrel(kestrel =>
{
    //通过配置文件来获取
    //测试主机ip
    //kestrel.Listen(IPAddress.Parse("172.20.64.1"),5196);

    //容器ip192.168.0.5
    //kestrel.Listen(IPAddress.Parse("192.168.0.5"), 5196);

    //容器ip192.168.0.8
    //kestrel.Listen(IPAddress.Parse("192.168.0.8"), 5197);

    kestrel.Listen(IPAddress.Parse(basiccfg.Ip), basiccfg.Port);
});

//builder.Services.AddDbContext<MyDbContext>();

var app = builder.Build();

app.UsArsExceptionMiddleware();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors("*");
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

app.MapGet("/", context => Task.Run(() => context.Response.Redirect("/swagger")));
app.Map("/healthCheck", builder => builder.Run(context => context.Response.WriteAsync("ok")));

app.Run();
