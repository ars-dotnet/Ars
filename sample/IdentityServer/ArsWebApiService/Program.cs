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
using Ars.Common.Tool.Configs;
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

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

var arsbuilder =
    builder.Services
    .AddArserviceCore(builder, config =>
    {
        var idsconfig = builder.Configuration.GetSection(nameof(ArsIdentityClientConfiguration)).Get<ArsIdentityClientConfiguration>();

        config.AddArsIdentityClient(configureOptions: options =>
        {
            options.Authority = idsconfig.Authority;
            options.ApiName = idsconfig.ApiName;
            options.RequireHttpsMetadata = idsconfig.RequireHttpsMetadata;

            if (idsconfig.RequireHttpsMetadata)
            {
                var httpClientHandler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Manual,
                    SslProtocols = SslProtocols.Tls12,
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                };
                httpClientHandler.ClientCertificates.Add(
                    Certificate.Get(idsconfig.CertificatePath, idsconfig.CertificatePassWord));

                options.JwtBackChannelHandler = httpClientHandler;
            }

            //for signalr token
            options.TokenRetriever = new Func<HttpRequest, string>(req =>
            {
                var fromHeader = TokenRetrieval.FromAuthorizationHeader();
                var fromQuery = TokenRetrieval.FromQueryString();
                return fromHeader(req) ?? fromQuery(req);
            });
        });

        config.AddArsRedis(provider =>
        {
            provider.ConfigureAll(cacheoption =>
            {
                cacheoption.DefaultSlidingExpireTime = TimeSpan.FromMinutes(10);
            });
        });

        config.AddArsSignalR(config =>
        {
            config.CacheType = 0;
            config.UseMessagePackProtocol = true;
        });
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
        option.SlidingExpireTime = TimeSpan.FromDays(1);
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(cors =>
{
    cors.AddPolicy("*", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod()
            .WithOrigins("https://172.20.64.1:7096", "http://172.20.64.1:5133");
    });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ArsWebApiService", Version = "v1" });

    var idscfg = builder.Configuration.GetSection(nameof(ArsIdentityClientConfiguration)).Get<ArsIdentityClientConfiguration>();
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

builder.WebHost.UseKestrel(kestrel =>
{
    //通过配置文件来获取
    //测试主机ip
    //kestrel.Listen(IPAddress.Parse("172.20.64.1"),5196);

    //容器ip192.168.0.5
    //kestrel.Listen(IPAddress.Parse("192.168.0.5"), 5196);

    //容器ip192.168.0.8
    //kestrel.Listen(IPAddress.Parse("192.168.0.8"), 5197);

    var basicfg = builder.Configuration.GetSection(nameof(ArsBasicConfiguration)).Get<ArsBasicConfiguration>();
    kestrel.Listen(IPAddress.Parse(basicfg.Ip), basicfg.Port);
});

//builder.Services.AddDbContext<MyDbContext>();

builder.Services.AddScoped<IHubSendMessage, MyWebHub>();
builder.Services.AddScoped<IWebServices,WebServices>();

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
app.MapHub<MyWebHub>("/ars/web/hub");
app.MapHub<ArsAndroidHub>("/ars/android/hub");

app.MapGet("/", context => Task.Run(() => context.Response.Redirect("/swagger")));
app.Map("/healthCheck", builder => builder.Run(context => context.Response.WriteAsync("ok")));

//app.UseSoapEndpoint<IWebServices>("/StudentService.asmx", new BasicHttpBinding(), SoapSerializer.XmlSerializer);
((IApplicationBuilder)app).UseSoapEndpoint<IWebServices>("/WebServices.asmx", new SoapEncoderOptions(), serializer : SoapSerializer.XmlSerializer);

app.Run();
