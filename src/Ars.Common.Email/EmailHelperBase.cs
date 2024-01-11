using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Commom.Email
{
    public abstract class EmailHelperBase : IEmailHelper
    {
        public abstract Task ReceiveAsync();

        public virtual Task SendAsync(MailMessage message, bool normalize = true)
        {
            if (message is null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (message.From == null) 
            {
                throw new ArgumentNullException(nameof(message.From));
            }
            if (message.To == null)
            {
                throw new ArgumentNullException(nameof(message.To));
            }

            if (normalize)
            {
                NormalizeMail(message);
            }

            return SendAsync(message);
        }

        protected abstract Task SendAsync(MailMessage mimeMessage);

        protected virtual void NormalizeMail(MailMessage message)
        {
            if (message.HeadersEncoding == null)
                message.HeadersEncoding = Encoding.UTF8;
            if (message.SubjectEncoding == null)
                message.SubjectEncoding = Encoding.UTF8;
            if (message.BodyEncoding == null)
                message.BodyEncoding = Encoding.UTF8;
        }
    }
}
