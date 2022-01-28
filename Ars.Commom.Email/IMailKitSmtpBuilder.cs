using Ars.Commom.Email.Configuration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Email
{
    public interface IMailKitSmtpBuilder : IDisposable
    {
        Task<SmtpClient> BuildAsync();
    }

    public class MailKitSmtpBuilder : IMailKitSmtpBuilder
    {
        private readonly IOptions<SmtpEmailSenderConfiguration> _options;
        public MailKitSmtpBuilder(IOptions<SmtpEmailSenderConfiguration> options)
        {
            _options = options;
        }

        private SmtpClient? _smtpClient;
        public async Task<SmtpClient> BuildAsync()
        {
            _smtpClient = new SmtpClient();

            try
            {
                await Connect();
                return _smtpClient;
            }
            catch
            {
                await _smtpClient.DisconnectAsync(true);
                throw;
            }

        }

        protected virtual async Task Connect() 
        {
            await _smtpClient!.ConnectAsync(_options.Value.Host,_options.Value.Port, GetSecureSocketOption());

            if (_options.Value.UseDefaultCredentials)
            {
                return;
            }

            await _smtpClient.AuthenticateAsync(
                _options.Value.UserName,
                _options.Value.Password
            );
        }

        protected virtual SecureSocketOptions GetSecureSocketOption()
        {
            return _options.Value.EnableSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;
        }

        public void Dispose()
        {
            _smtpClient?.Dispose();
        }
    }
}
