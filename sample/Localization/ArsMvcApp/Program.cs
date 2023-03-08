using Ars.Commom.Host.Extension;
using ArsMvcApp;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ars.Common.Redis;
using Ars.Common.Redis.RedisExtension;
using Ars.Common.Redis.Extension;
using Ars.Common.Core.Localization.Extension;

var builder = WebApplication.CreateBuilder(args);


var provider = builder.Services.
    AddArserviceCore(builder.Host,config => 
    {
        config.AddArsRedis(provider =>
        {
            provider.ConfigureAll(cacheoption =>
            {
                cacheoption.DefaultSlidingExpireTime = TimeSpan.FromMinutes(10);
            });
        });
        config.AddArsLocalization();
    });

//builder.Services.Configure<Configs>(builder.Configuration.GetSection(nameof(Configs)));
//builder.Services.Configure<Configs>(option =>
//{
//    option.Name = "xx";
//    option.Age = 111;
//});
//var a = builder.Services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<IOptions<Configs>>();

builder.Services.AddTransient<IUserAppService, User>();
builder.Services.AddTransient<UserBase, User>();
builder.Services.AddTransient<User>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseArsCore();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseArsLocalization();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();