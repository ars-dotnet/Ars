using Ars.ArsWebApiGrpcService.HttpApi;
using Ars.Commom.Host.Extension;
using Ars.Common.Consul.Extension;
using Ars.Common.Consul.IApplicationBuilderExtension;
using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.IdentityServer4.Extension;
using Ars.Common.IdentityServer4.Options;
using Ars.Common.Tool.Extension;
using GrpcClient.ApmLogger;
using GrpcClients;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using System.Net.Http;
using Ars.Common.RpcClientCore.Extensions;
using Ars.ArsWebApiService.HttpApi;
using Ars.Common.Core.Extensions;
using Ars.Common.Core.AspNetCore.Swagger;

Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", "SkyAPM.Agent.AspNetCore");
Environment.SetEnvironmentVariable("SKYWALKING__SERVICENAME", "grpclient");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddArsSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ArsGrpcClient", Version = "v1" });

    var idscfg = builder.Configuration
        .GetSection(nameof(ArsIdentityClientConfiguration))
        .Get<ArsIdentityClientConfiguration>();
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
                    { "grpcapi-scope","��Ȩ��д����" }
                }
            }
        }
    });

    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ArsGrpcClient.xml");
    if (File.Exists(path))
    {
        c.IncludeXmlComments(path);
    }

    //ö����ʾΪ�ַ���
    c.SchemaFilter<EnumSchemaFilter>();
    //����AuthorizeAttributea�����Ƿ���Ҫ��Ȩ����
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.Services
    .AddArserviceCore(builder, config =>
    {
        config
            .AddArsIdentityClient()
            .AddArsConsulDiscoverClient()
            .AddArsHttpApi<IWeatherForecastHttpApi>(option =>
            {
                //����arsĬ�ϵ�httpclient
                option.UseArsHttpClient = true;
                //����arsĬ�ϵ�HttpsMessageHandler
                option.UseHttps = true;
                //����arsĬ�ϵĲ���
                option.UseHttpClientCustomPolicy = true;
            }, builder => 
            {
                builder.ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(10));
            })
            .AddArsHttpApi<IDbHttpApi>(configureBuilder:builder => 
            {
                //�Զ������
                builder.AddArsTransientHttpErrorPolicy();
                //�Զ���HttpsMessageHandler
                builder.ConfigureArsPrimaryHttpsMessageHandler();
            })
            .AddArsHttpApi(typeof(IRpcHttpApi), configureBuilder: builder =>
            {
                //�Զ������
                builder.AddArsTransientHttpErrorPolicy();
                //�Զ���HttpsMessageHandler
                builder.ConfigureArsPrimaryHttpsMessageHandler();

                builder.ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(10));
            });

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

builder.Services.AddTransient<MyDelegatingHandler>();
builder.Services.AddHttpClient("test").AddHttpMessageHandler<MyDelegatingHandler>();

var app = builder.Build();

app.UsArsExceptionMiddleware();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseArsCore();

app.MapControllers();

app.MapGet("/", context => Task.Run(() => context.Response.Redirect("/swagger")));

app.Run();
