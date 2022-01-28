using Microsoft.Extensions.DependencyInjection;
using MyArsenal.Commom.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Email.Configuration
{
    public class SmtpEmailExtensionConfig : IConfigExtension
    {
        private readonly Action<SmtpEmailSenderConfiguration> _action;
        public SmtpEmailExtensionConfig([NotNull] Action<SmtpEmailSenderConfiguration> action)
        {
            _action = action;
        }

        public void AddService(IServiceCollection services)
        {
            services.Configure(_action);
            services.AddTransient<IMailKitSmtpBuilder, MailKitSmtpBuilder>();
            services.AddTransient<IEmailHelper, MailKitEmailHelper>();
        }
    }
}
