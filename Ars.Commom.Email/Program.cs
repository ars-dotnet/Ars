// See https://aka.ms/new-console-template for more information

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

using var email = new MimeMessage();
//email.From.Add(MailboxAddress.Parse("yangbo@geekbuy.com"));
//email.To.Add(MailboxAddress.Parse("1432507436@qq.com"));
email.From.Add(MailboxAddress.Parse("1432507436@qq.com"));
email.To.Add(MailboxAddress.Parse("1432507436@qq.com"));
email.Subject = "Test Email Subject";
email.Body = new TextPart(TextFormat.Html) { Text = "<h1>hello word</h1>" };

// send email
using var smtp = new SmtpClient();

smtp.Connect("smtp.qq.com", 587, SecureSocketOptions.StartTls);
//smtp.Connect("smtp.exmail.qq.com", 587, SecureSocketOptions.StartTls);


//1432507436@qq.com xmyezgingmucbacd
//yangbo@geekbuy.com PpaDsH3Aw4bZEzHj

//Note: only needed if the SMTP server requires authentication
//需用授权码而不是邮箱登录密码
smtp.Authenticate("1432507436@qq.com", "xmyezgingmucbacd");
//smtp.Authenticate("yangbo@geekbuy.com", "PpaDsH3Aw4bZEzHj");

smtp.Send(email);
smtp.Disconnect(true);
