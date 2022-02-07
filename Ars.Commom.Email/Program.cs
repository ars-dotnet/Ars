// See https://aka.ms/new-console-template for more information

using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System.Text;

using var email = new MimeMessage();
//email.From.Add(MailboxAddress.Parse("yangbo@geekbuy.com"));
//email.To.Add(MailboxAddress.Parse("1432507436@qq.com"));
email.From.Add(MailboxAddress.Parse("769581834@qq.com"));
email.To.Add(MailboxAddress.Parse("1432507436@qq.com"));
//email.From.Add(MailboxAddress.Parse("1432507436@qq.com"));
//email.To.Add(MailboxAddress.Parse("769581834@qq.com"));
email.Subject = "ars";
email.Body = new TextPart(TextFormat.Html) { Text = "<h1>2022.1.29.009 new</h1>" };
string mid = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString().Replace("-", "")));
//email.MessageId = mid;
//email.ResentMessageId = mid;
// send email
using var smtp = new SmtpClient();

smtp.Connect("smtp.qq.com", 587, SecureSocketOptions.StartTls);
//smtp.Connect("smtp.exmail.qq.com", 587, SecureSocketOptions.StartTls);


//1432507436@qq.com xmyezgingmucbacd
//yangbo@geekbuy.com PpaDsH3Aw4bZEzHj

//Note: only needed if the SMTP server requires authentication
//需用授权码而不是邮箱登录密码
smtp.Authenticate("769581834@qq.com", "ttwhbpuneetybdig");
//smtp.Authenticate("1432507436@qq.com", "xmyezgingmucbacd");
//smtp.Authenticate("yangbo@geekbuy.com", "PpaDsH3Aw4bZEzHj");

smtp.Send(email);
smtp.Disconnect(true);

Console.WriteLine(email.MessageId);
Console.ReadLine();
