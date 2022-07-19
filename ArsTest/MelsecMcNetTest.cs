using HslCommunication;
using HslCommunication.Profinet.Melsec;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsTest
{
    public class MelsecMcNetTest
    {
        [Fact]
        public void Test() 
        {
            // 实例化对象，指定PLC的ip地址和端口号
            //MelsecMcNet melsecMc = new MelsecMcNet("192.168.110.66", 5002);
            MelsecMcNet melsecMc = new MelsecMcNet("192.0.1.254", 8001);
            melsecMc.ConnectTimeOut = 2000; // 网络连接的超时时间
            melsecMc.NetworkNumber = 0x00;  // 网络号
            melsecMc.NetworkStationNumber = 0x00; // 网络站号
            // 连接对象
            OperateResult connect = melsecMc.ConnectServer();
            Assert.True(connect.IsSuccess);
        }
    }
}
