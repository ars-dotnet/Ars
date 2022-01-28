using System;
using System.Collections.Generic;
using System.Linq;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using MimeKit;
using Microsoft.Extensions.Logging;
using MailKit.Net.Imap;
using MailKit;

namespace Ars.Commom.Email
{
    public class MailKitEmailHelper : EmailHelperBase
    {
        private readonly IMailKitSmtpBuilder _mailKitSmtpBuilder;
        public MailKitEmailHelper(IMailKitSmtpBuilder mailKitSmtpBuilder)
        {
            _mailKitSmtpBuilder = mailKitSmtpBuilder;
        }

        public override Task SendAsync(MailMessage message, bool normalize = true)
        {
            return base.SendAsync(message, normalize);
        }

        protected override async Task SendAsync(MailMessage mimeMessage)
        {
            using var client = await BuildSmtpClient();
            var message = MimeMessage.CreateFromMailMessage(mimeMessage);
            message.MessageId = Guid.NewGuid().ToString();

            Console.WriteLine("messageid:" + message.MessageId);

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return;
        }

        protected virtual Task<SmtpClient> BuildSmtpClient() 
        {
            return _mailKitSmtpBuilder.BuildAsync();
        }

        public override async Task ReceiveAsync()
        {
            using (var client = new ImapClient())
            {
                // For demo-purposes, accept all SSL certificates
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                await client.ConnectAsync("imap.qq.com", 993, true);

                await client.AuthenticateAsync("1432507436@qq.com", "xmyezgingmucbacd");

                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);

                Console.WriteLine("Total messages: {0}", inbox.Count);
                Console.WriteLine("Recent messages: {0}", inbox.Recent);

                for (int i = 0; i < inbox.Count; i++)
                {
                    var message = await inbox.GetMessageAsync(i);
                    Console.WriteLine("Subject: {0}", message.Subject);
                }

                await client.DisconnectAsync(true);
            }

            return;
        }
    }
}
