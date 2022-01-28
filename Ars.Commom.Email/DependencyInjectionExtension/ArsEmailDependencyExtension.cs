using Ars.Commom.Email.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyArsenal.Commom.Tool;
using System.Diagnostics.CodeAnalysis;

namespace Ars.Commom.Email.DependencyInjectionExtension
{
    public static class ArsEmailDependencyExtension
    {
        public static void AddArsEmail(this ArsBaseConfig services,[NotNull]Action<SmtpEmailSenderConfiguration> action) 
        {
            SmtpEmailExtensionConfig config = new SmtpEmailExtensionConfig(action);
            services.AddConfigExtension(config);
        }
    }
}
