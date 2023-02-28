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

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

var arsbuilder = 
    builder.Services
    .AddArserviceCore(builder.Host)
    .AddArsIdentityClient()
    .AddArsDbContext<MyDbContext>();
builder.Services.AddArsHttpClient();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(cors =>
{
    cors.AddPolicy("*", policy =>
    {
        policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
    });
});

var idscfg = builder.Services.BuildServiceProvider().GetRequiredService<IArsIdentityClientConfiguration>();

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

//builder.WebHost.UseUrls("http://127.0.0.1:5197");

//builder.WebHost.UseKestrel(kestrel =>
//{
//    kestrel.Listen(IPAddress.Parse("192.168.110.67"), 5197);
//});

//builder.Services.AddDbContext<MyDbContext>();

var app = builder.Build();

app.UsArsExceptionMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("*");
app.UseStaticFiles();

app.UseArsCore();
app.MapControllers();

app.Run();
