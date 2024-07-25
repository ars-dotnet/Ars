using Ars.Commom.Core;
using Ars.Common.Core.Configs;
using Ars.Common.Core.Localization.Options;
using Ars.Common.Core.Localization.ValidProvider;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;

namespace Ars.Common.Core.Localization.Extension
{
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// 浏览器请求时，添加消息头Accept-Language
        /// en-US/zh-CN 使用不同时区
        /// </summary>
        /// <param name="arsServiceBuilder"></param>
        /// <returns></returns>
        public static IArsWebApplicationBuilder AddArsLocalization(
            this IArsWebApplicationBuilder arsServiceBuilder)
        {
            var arsLocalizationOption = arsServiceBuilder.Configuration
                 .GetSection(nameof(ArsLocalizationConfiguration))
                 .Get<ArsLocalizationConfiguration>()
                 ??
                 new ArsLocalizationConfiguration() { Cultures = new[] { "en-US", "zh-CN" } };

            arsServiceBuilder.ArsConfiguration.ArsLocalizationConfiguration ??= arsLocalizationOption;

            arsServiceBuilder.ArsConfiguration.AddArsAppExtension(new ArsLocalizationAppExtension());

            var services = arsServiceBuilder.Services;

            services.AddSingleton<IArsLocalizationConfiguration>(_ => arsLocalizationOption);

            services.AddLocalization(option =>
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

            services.AddSingleton<IArstringLocalizer, ArstringLocalizer>();
            return arsServiceBuilder;
        }

        private static IMvcBuilder AddArsViewLocalization(this IMvcBuilder mvcBuilder, ArsLocalizationConfiguration arsLocalizationOption)
        {
            mvcBuilder.AddMvcOptions(option => option.ModelMetadataDetailsProviders.Add(new ValidationMetadataLocalizationProvider()));
            if (arsLocalizationOption.IsAddViewLocalization)
                mvcBuilder.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix);//AddViewLocalization adds support for localized view files.
                                                                                          //In this sample view localization is based on the view file suffix. For example "fr" in the Index.fr.cshtml file.
            if (arsLocalizationOption.IsAddDataAnnotationsLocalization)
                mvcBuilder.AddDataAnnotationsLocalization(options =>  //AddDataAnnotationsLocalization adds support for localized DataAnnotations validation messages through IStringLocalizer abstractions.
                {
                    options.DataAnnotationLocalizerProvider =
                         (_, factory) => factory.Create(nameof(ArshareResource), new AssemblyName(Assembly.GetEntryAssembly()!.FullName!).Name!);
                });

            return mvcBuilder;
        }
    }
}
