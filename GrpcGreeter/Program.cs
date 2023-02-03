using Ars.Commom.Host.Extension;
using Ars.Common.Consul.IApplicationBuilderExtension;
using GrpcGreeter.greet;
using GrpcGreeter.Services;
using GrpcGreeterService.Protos;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();

var arsbuilder = builder.Services.AddArserviceCore(builder.Host);
arsbuilder.AddArsConsulRegisterServer(
    //option => 
    //{
    //    option.ConsulAddress = "http://127.0.0.1:8500";
    //    option.ServiceName = "apigrpc";
    //    option.ServiceIp = "127.0.0.1";
    //    option.ServicePort = 7903;
    //}
);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<GreeterService>();
app.MapGrpcService<HealthCheckService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

//app.UseArsConsul();

app.Run();
