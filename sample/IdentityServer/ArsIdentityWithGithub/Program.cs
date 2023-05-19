using Ars.Commom.Host.Extension;
using Ars.Common.AutoFac;
using Ars.Common.Host;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using MyIdentityWithGithub;
using System.Globalization;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization();

//builder.Services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

// They also allow you to have localised View files (so you can have Views with names like MyView.fr.cshtml) and inject the IViewLocalizer
// to allow you to use localisation in your view files.
builder.Services.AddMvc().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,
                options => options.ResourcesPath = "Resource").
                AddDataAnnotationsLocalization();
                

builder.Services.Configure<RequestLocalizationOptions>(
        opts =>
        {
            var supportedCultures = new List<CultureInfo>
            {
                new CultureInfo("en-GB"),
                new CultureInfo("en-US"),
                new CultureInfo("en"),
                new CultureInfo("fr-FR"),
                new CultureInfo("fr"),
                new CultureInfo("zh-Hans")
            };

            opts.DefaultRequestCulture = new RequestCulture("en-GB");
            // Formatting numbers, dates, etc.
            opts.SupportedCultures = supportedCultures;
            // UI strings that we have localized.
            opts.SupportedUICultures = supportedCultures;
        });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();

var Configuration = builder.Configuration;
builder.Services.AddAuthentication(option =>
{
    option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(option =>
{
    option.LoginPath = "/signin";
    option.LogoutPath = "/signout";
})
.AddGitHub(options => 
{
    options.ClientId = Configuration["GitHub:ClientId"];
    options.ClientSecret = Configuration["GitHub:ClientSecret"];
    options.EnterpriseDomain = Configuration["GitHub:EnterpriseDomain"];
    options.Scope.Add("user:email");

    options.CallbackPath = "/myars/signin-github";
});

builder.Services.AddArserviceCore(builder);

var a = Directory.GetCurrentDirectory();
var b = AppDomain.CurrentDomain.BaseDirectory;
var location = Assembly.GetExecutingAssembly().Location;
if (location != null)
{
    string c = Directory.GetParent(location)!.FullName;//∫Õbœ‡µ»
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options!.Value);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/12", Greet);
static string Greet(int a) => a.ToString();

app.Run();
