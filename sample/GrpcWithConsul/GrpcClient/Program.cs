using Ars.Commom.Host.Extension;
using Ars.Common.Consul.IApplicationBuilderExtension;
using GrpcClients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddArserviceCore(builder.Host)
    .AddArsConsulDiscoverClient();

builder.Services.AddScoped<IChannelManager, ChannelManager>();
builder.Services.AddSingleton<IChannelProvider, ChannelProvider>();

builder.Services.AddSingleton<ChannelService>();
builder.Services.AddHostedService(pro => pro.GetRequiredService<ChannelService>());

builder.Services.AddSingleton<ChannelPublish>();
builder.Services.AddHostedService(pro => pro.GetRequiredService<ChannelPublish>());

builder.Services.AddSingleton<ChannelPublish2>();
builder.Services.AddHostedService(pro => pro.GetRequiredService<ChannelPublish2>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseArsCore();

app.MapControllers();

app.Run();
