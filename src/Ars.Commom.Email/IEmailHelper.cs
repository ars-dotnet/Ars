using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Email
{
    public interface IEmailHelper
    {
        public Task SendAsync(MailMessage message,bool normalize = true);

        public Task ReceiveAsync();
    }
}
