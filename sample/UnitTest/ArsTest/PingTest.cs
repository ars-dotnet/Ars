using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class PingTest
    {
        [Fact]
        public void Test1() 
        {
            //Ping 实例对象;
            Ping pingSender = new Ping();
            //ping选项;
            PingOptions options = new PingOptions();
            options.DontFragment = true;
            string data = "ping test data";
            byte[] buf = Encoding.ASCII.GetBytes(data);
            //调用同步send方法发送数据，结果存入reply对象;
            PingReply reply = pingSender.Send(IPAddress.Parse("127.0.0.1"), 120, buf, options);

            if (reply.Status == IPStatus.Success)
            {
                Console.WriteLine("主机地址::" + reply.Address);
                Console.WriteLine("往返时间::" + reply.RoundtripTime);
                Console.WriteLine("生存时间TTL::" + reply.Options.Ttl);
                Console.WriteLine("缓冲区大小::" + reply.Buffer.Length);
                Console.WriteLine("数据包是否分段::" + reply.Options.DontFragment);
            }
        }
    }
}
