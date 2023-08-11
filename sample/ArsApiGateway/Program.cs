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
});

builder.Services.AddCors(cors =>
{
    cors.AddPolicy("*", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowCredentials()
            .AllowAnyMethod()
            .WithOrigins("http://127.0.0.1:63042"); //ws¿çÓòÅäÖÃ
    });
});

builder.WebHost.UseArsKestrel(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseCors("*");

app.UseArsCore();

app.MapControllers();

app.MapGet("/",httpcontext => Task.Run(() => { httpcontext.Response.Redirect("/swagger"); }));

app.Run();
