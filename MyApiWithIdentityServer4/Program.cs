using Ars.Commom.Host.Extension;
using Ars.Common.IdentityServer4.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApiWithIdentityServer4;
using Ars.Common.EFCore.Extension;
using Ars.Common.Core.AspNetCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

var arsbuilder = builder.Services.AddArserviceCore(builder.Host);
arsbuilder.AddArsIdentityClient(configureOptions : option =>
{
    option.Authority = "http://localhost:5105";
    option.ApiName = "grpcapi";
    option.RequireHttpsMetadata = false;
});
arsbuilder.AddArsDbContext<MyDbContext>();

arsbuilder.Services.ServiceCollection.AddScoped<ITestScopeService,TestScopeService>();
var aa = builder.Configuration.GetSection("Config").Get<Config>();

builder.Services.AddSingleton<IOptions<IConfig>>(new OptionsWrapper<IConfig>(aa));
var mm = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<IConfig>>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
//builder.Services.AddDbContext<MyDbContext>();

var app = builder.Build();

app.UseArsCore();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseArsIdentityClient();

app.MapControllers();

app.Run();
