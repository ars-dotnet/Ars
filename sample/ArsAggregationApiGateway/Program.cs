using Ars.Commom.Host.Extension;
using Ars.Common.Ocelot.Extension;
using Ars.Common.Host.Extension;
using Microsoft.OpenApi.Models;
using Ars.Common.Tool.Swagger;
using Ars.Common.IdentityServer4.Options;
using Ars.Common.IdentityServer4.Extension;
using Ars.Common.Consul.Extension;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddArserviceCore(builder,config => 
{
    config.AddArsOcelot();

    config.AddArsIdentityClient();

    config.AddArsConsulDiscoverClient();
});

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

builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ars Gateway Api", Version = "v1" });

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

    string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ArsAggregationApiGateway.xml");
    if (File.Exists(path))
    {
        c.IncludeXmlComments(path);
    }

    //枚举显示为字符串
    c.SchemaFilter<EnumSchemaFilter>();
    //根据AuthorizeAttributea分配是否需要授权操作
    c.OperationFilter<SecurityRequirementsOperationFilter>();
});

builder.WebHost.UseArsKestrel(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("*");

app.UseArsCore();

app.MapControllers();

app.MapGet("/",httpcontext => Task.Run(() => { httpcontext.Response.Redirect("/swagger"); }));

app.Run();
