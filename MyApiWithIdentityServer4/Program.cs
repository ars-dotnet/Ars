using Ars.Commom.Host.Extension;
using Ars.Common.IdentityServer4.Extension;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.

var arsbuilder = builder.Services.AddArserviceCore(builder);
arsbuilder.AddArsIdentityServerAuthentication(configureOptions : option =>
{
    option.Authority = "http://localhost:7207";
    option.ApiName = "defaultApi";
    option.RequireHttpsMetadata = false;
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
