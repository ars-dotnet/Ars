using Ars.Commom.Host.Extension;
using Ars.Common.IdentityServer4.Extension;
using MyIdentittServer4;
using MyIdentittServer4.Configs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var provider = builder.Services.AddArserviceCore(builder);
provider.AddArsIdentityServer();
//builder.Services.AddMyAuthentication();

builder.Services.AddCors(options =>
{
    options.AddPolicy("test", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("test");

app.UseHttpsRedirection();

app.UseRouting();

app.UseArsIdentityServer();

app.MapControllers();

app.Run();
