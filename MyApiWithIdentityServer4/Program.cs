using Ars.Commom.Host.Extension;
using Ars.Common.IdentityServer4.Extension;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyApiWithIdentityServer4;
using Ars.Common.EFCore.Extension;
using Ars.Common.Core.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

var arsbuilder = builder.Services.AddArserviceCore(builder);
arsbuilder.AddArsIdentityClient(configureOptions : option =>
{
    option.Authority = "http://localhost:5105";
    option.ApiName = "defaultApi";
    option.RequireHttpsMetadata = false;
});
arsbuilder.AddArsDbContext<MyDbContext>(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
//builder.Services.AddDbContext<MyDbContext>();

var app = builder.Build();

app.UseArsCore(option =>
{
    option.UnitOfWorkDefaultConfiguration.TimeOut = TimeSpan.FromMinutes(1);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseArsIdentityClient();

app.MapControllers();

app.Run();
