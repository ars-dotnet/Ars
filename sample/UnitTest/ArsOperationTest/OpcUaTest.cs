using ConsoleAppNew;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsOperationTest
{
    public class OpcUaTest
    {
        const string connectString = "opc.tcp://127.0.0.1:62541/Quickstarts/ReferenceServer";

        [Fact]
        public async void Test1() 
        {
            OpcUaClient opcUaClient = new OpcUaClient();

            await opcUaClient.ConnectServer(connectString);

            var data = await opcUaClient.ReadNodeAsync<long>("ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/Int64");
            var data1 = await opcUaClient.ReadNodeAsync<long>("ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/Int64");
            var data2 = await opcUaClient.ReadNodeAsync<short>("ns=2;Devices/分厂二/车间三/ModbusTcp客户端02/温度");

            var data3 = await opcUaClient.ReadNodeAsync<short[]>("ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/Int16");

            opcUaClient.Disconnect();

            Assert.True(6665 == data);
            Assert.True(6666 == data1);
            Assert.True(38 == data2);
        }

        /// <summary>
        /// 测试多节点写
        /// </summary>
        [Fact]
        public async void TestWriteMultipleNodes()
        {
            OpcUaClient opcUaClient = new OpcUaClient();

            await opcUaClient.ConnectServer(connectString);

            var res = opcUaClient.WriteNodes(
                new string[]
                {
                    "ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/Float",
                    "ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/Double",
                },
                new object[] 
                {
                    12345.6f,
                    345.6677
                });

            opcUaClient.Disconnect();

            Assert.True(true == res);
        }

        /// <summary>
        /// 测试单节点写多数组
        /// </summary>
        [Fact]
        public async void TestWriteArray()
        {
            OpcUaClient opcUaClient = new OpcUaClient();

            await opcUaClient.ConnectServer(connectString);

            var data = opcUaClient.WriteNode(
                "ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/Int16",
                new int[]
                {
                    6667,
                    7778,
                    9999
                });

            opcUaClient.Disconnect();

            Assert.True(true == data);
        }
    }
}
