using Ars.Commom.Host.Extension;
using Ars.Common.IdentityServer4.Extension;
using Ars.Common.Localization.Extension;
using Ars.Common.Localization.IServiceCollectionExtension;
using Ars.Common.Localization.ValidProvider;
using ArsMvcApp;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ars.Common.Redis;

var builder = WebApplication.CreateBuilder(args);


var provider = builder.Services.AddArserviceCore(builder);
provider.AddArsLocalization(option =>
{
    option.IsAddViewLocalization = true;
    option.Cultures = option.Cultures.Concat(new[] { "en-GB", "en", "fr-FR", "fr" });
});
//provider.AddArsIdentityServer4();

builder.Services.Configure<User>(builder.Configuration.GetSection(nameof(User)));

builder.Services.AddTransient<IUserAppService, User>();
builder.Services.AddTransient<UserBase, User>();
builder.Services.AddTransient<User>();

provider.AddArsRedis();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseArsLocalization();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseArsRedis(options =>
{
    options.RedisConnection = "192.168.2.102";
    options.DefaultDB = 1;
}, config =>
{
    config.ConfigureAll(cacheoption =>
    {
        cacheoption.DefaultAbsoluteExpireTime = DateTime.Now.AddMinutes(10);
    });
});
app.Run();


class Test { 
    public string ConnectionString { get; set; }
}
