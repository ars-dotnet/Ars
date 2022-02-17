using Ars.Commom.Host;
using Ars.Common.Localization.options;
using Ars.Common.Localization.ValidProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Localization.IServiceCollectionExtension
{
    public static class IServiceCollectionExtension
    {
        public static IArsServiceBuilder AddArsLocalization(this IArsServiceBuilder arsServiceProvider, Action<ArsLocalizationOption>? action = null)
        {
            ArsLocalizationOption arsLocalizationOption = new ArsLocalizationOption();
            action?.Invoke(arsLocalizationOption);
            var services = arsServiceProvider.Services.ServiceCollection;
            services.AddLocalization(
                option =>
                     option.ResourcesPath = arsLocalizationOption.ResourcesPath);

            IMvcBuilder builder;
            if (arsLocalizationOption.IsAddViewLocalization)
                builder = services.AddControllersWithViews();
            else
                builder = services.AddControllers();

            builder.AddArsViewLocalization(arsLocalizationOption);

            services.Configure<RequestLocalizationOptions>(option =>
            {
                CultureInfo[] cultureInfos = 
                    arsLocalizationOption.Cultures
                        .Distinct()
                        .Select(r => new CultureInfo(r)).ToArray();

                option.DefaultRequestCulture = new RequestCulture(arsLocalizationOption.DefaultRequestCulture);
                // Formatting numbers, dates, etc.
                option.SupportedCultures = cultureInfos;
                // UI strings that we have localized.
                option.SupportedUICultures = cultureInfos;
                //The Content-Language header can be added by setting the property ApplyCurrentCultureToResponseHeaders.
                option.ApplyCurrentCultureToResponseHeaders = true;
            });

            services.AddTransient<IArstringLocalizer, ArstringLocalizer>();

            return arsServiceProvider;
        }

        private static IMvcBuilder AddArsViewLocalization(this IMvcBuilder mvcBuilder, ArsLocalizationOption arsLocalizationOption)
        {
            mvcBuilder.AddMvcOptions(option => option.ModelMetadataDetailsProviders.Add(new ValidationMetadataLocalizationProvider()));
            if (arsLocalizationOption.IsAddViewLocalization)
                mvcBuilder.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);//AddViewLocalization adds support for localized view files.
                                                                                          //In this sample view localization is based on the view file suffix. For example "fr" in the Index.fr.cshtml file.
            if (arsLocalizationOption.IsAddDataAnnotationsLocalization)
                mvcBuilder.AddDataAnnotationsLocalization(options =>  //AddDataAnnotationsLocalization adds support for localized DataAnnotations validation messages through IStringLocalizer abstractions.
                {
                    options.DataAnnotationLocalizerProvider = 
                         (_, factory) => factory.Create(nameof(ArshareResource),new AssemblyName(Assembly.GetEntryAssembly()!.FullName!).Name!);
                });

            return mvcBuilder;
        }
    }
}
