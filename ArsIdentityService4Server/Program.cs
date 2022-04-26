using Ars.Commom.Host.Extension;
using Ars.Common.IdentityServer4.Extension;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var arsbuilder = builder.Services.AddArserviceCore(builder);
arsbuilder.AddArsIdentityServer();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseArsIdentityServer();

app.MapRazorPages();

app.Run();
