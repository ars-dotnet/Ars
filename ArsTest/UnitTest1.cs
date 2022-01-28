using Microsoft.Extensions.DependencyInjection;
using Xunit;
using MyArsenal.Commom.Tool;
using Ars.Commom.Email.DependencyInjectionExtension;
using Ars.Commom.Email;
using System.Threading.Tasks;
using System;

namespace ArsTest
{
    public class UnitTest1
    {
        private IServiceCollection _services;
        public UnitTest1()
        {
            _services = new ServiceCollection();
            _services.AddArsConfig(option =>
            {
                option.AddArsEmail(eopt =>
                {
                    eopt.Host = "smtp.qq.com";
                    eopt.Port = 587;
                    eopt.EnableSsl = false;
                    eopt.UserName = "1432507436@qq.com";
                    eopt.Password = "xmyezgingmucbacd";
                    eopt.UseDefaultCredentials = false;
                });
            });
        }

        [Fact]
        public async Task TestSmptEmail()
        {
            var provider = _services.BuildServiceProvider().CreateScope().ServiceProvider;
            var helper = provider.GetRequiredService<IEmailHelper>();
            await helper.SendAsync(new System.Net.Mail.MailMessage("1432507436@qq.com", "1432507436@qq.com", "ars_project","ars_body"));
        }

        [InlineData("sex","boy")]
        [Theory]
        public async Task TestSmptEmailReceive(string name,string value) 
        {
            var provider = _services.BuildServiceProvider().CreateScope().ServiceProvider;
            var helper = provider.GetRequiredService<IEmailHelper>();

            try
            {
                await helper.ReceiveAsync();
            }
            catch (Exception e) 
            {

            }
            
        }
    }
}