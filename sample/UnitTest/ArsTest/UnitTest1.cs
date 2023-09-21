using Microsoft.Extensions.DependencyInjection;
using Xunit;
using System.Threading.Tasks;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Ars.Commom.Email;

namespace ArsTest
{
    public class UnitTest1
    {
        private IServiceCollection _services;
        public UnitTest1()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public async Task TestSmptEmail()
        {
            var provider = _services.BuildServiceProvider().CreateScope().ServiceProvider;
            var helper = provider.GetRequiredService<IEmailHelper>();
            await helper.SendAsync(new System.Net.Mail.MailMessage("1432507436@qq.com", "1432507436@qq.com", "ars_project","ars_body"));
        }

        [InlineData(123,"boy")]
        [Theory]
        public async Task TestSmptEmailReceive(int t1,string t2) 
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

        [Fact]
        public void TestSerializable()
        {
            string fileName = "testSerilizable";

            // Use a BinaryFormatter or SoapFormatter.
            IFormatter formatter = new BinaryFormatter();
            //IFormatter formatter = new SoapFormatter();
            SerializableTest test = new SerializableTest();
            test.SerializeItem(fileName, formatter); // Serialize an instance of the class.
            test.DeserializeItem(fileName, formatter); // Deserialize the instance.
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        [Fact]
        public void TestResource() 
        {

        }
    }

    [Serializable]
    public class SerializableTest : ISerializable
    {
        private string Name { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", "hello " + Name);
        }

        public SerializableTest()
        {

        }

        public SerializableTest(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetValue("name", typeof(string))?.ToString() ?? "";
        }


        public void SerializeItem(string fileName, IFormatter formatter)
        {
            // Create an instance of the type and serialize it.
            SerializableTest t = new SerializableTest();
            t.Name = "Tom";

            FileStream s = new FileStream(fileName, FileMode.Create);
            formatter.Serialize(s, t);
            s.Close();
        }

        public void DeserializeItem(string fileName, IFormatter formatter)
        {
            FileStream s = new FileStream(fileName, FileMode.Open);
            SerializableTest t = (SerializableTest)formatter.Deserialize(s);
            Console.WriteLine(t.Name);
        }
    }
}