using ArsMvcApp;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using System.Globalization;var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddMvcOptions(options =>
    {
        options.ModelMetadataDetailsProviders.Add(new ValidationMetadataLocalizationProvider());
    })
    //AddViewLocalization adds support for localized view files.
    //In this sample view localization is based on the view file suffix. For example "fr" in the Index.fr.cshtml file.
    .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix,options => options.ResourcesPath = "Resources")
                //AddDataAnnotationsLocalization adds support for localized DataAnnotations validation messages
                //through IStringLocalizer abstractions.
                .AddDataAnnotationsLocalization(options => {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResource));
                });

//AddLocalization adds the localization services to the services container.
//The code bellow also sets the resources path to "Resources".
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

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

            opts.DefaultRequestCulture = new RequestCulture("en-US", "en-US");
            // Formatting numbers, dates, etc.
            opts.SupportedCultures = supportedCultures;
            // UI strings that we have localized.
            opts.SupportedUICultures = supportedCultures;
            //The Content-Language header can be added by setting the property ApplyCurrentCultureToResponseHeaders.
            opts.ApplyCurrentCultureToResponseHeaders = true;
        });

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

var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(options!.Value);

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
