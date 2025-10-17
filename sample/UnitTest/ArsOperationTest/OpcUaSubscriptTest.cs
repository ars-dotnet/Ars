using ConsoleAppNew;
using Opc.Ua.Client;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ArsOperationTest
{
    public class OpcUaSubscriptTest
    {
        const string connectString = "opc.tcp://127.0.0.1:62541/Quickstarts/ReferenceServer";

        string[] combTags = new string[]
        {
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error1", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error2", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error3", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error4", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error5", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error6", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error7", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error8", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error9", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error10", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error11", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error12", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error13", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error14", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error15", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error16", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/error17", //D107

            //"ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/errorAll", //D107
            "ns=2;Devices/WorkFactory01/WorkShop01/MelsecTest/Int16", //D107
        };

        string[] RetLineTags = new string[]
        {
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error1", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error2", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error3", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error4", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error5", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error6", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error7", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error8", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error9", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error10", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error11", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error12", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error13", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error14", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error15", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error16", //D107
            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/error17", //D107

            //"ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/errorAll", //D107
            "ns=2;Devices/WorkFactory01/WorkShop02/ModbusTcpTest/Int16", //D107
        };

        /// <summary>
        /// 测试节点监听
        /// </summary>
        [Fact]
        public async void TestListeneCallBack()
        {
            OpcUaClient opcUaClient = new OpcUaClient();

            await opcUaClient.ConnectServer(connectString);

            //添加订阅
            //第一次读取会订阅到
            //PLC值发生变化会订阅到
            opcUaClient.AddSubscription("Comb", combTags, SubCallback);

            opcUaClient.AddSubscription("RetLine", RetLineTags, SubCallback);

            Console.ReadLine();
            Console.ReadKey();

            opcUaClient.Disconnect();
        }

        private static void SubCallback(string key, MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs args)
        {
            if (args.NotificationValue is MonitoredItemNotification notification && null != notification)
            {
                var showValue = notification.Value.WrappedValue.Value;

                Console.WriteLine($"Key:{monitoredItem.StartNodeId} Value:{showValue}");
            }
        }
    }
}
