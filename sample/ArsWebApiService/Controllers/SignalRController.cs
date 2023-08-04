using Ars.Common.SignalR.Sender;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ArsWebApiService.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SignalRController : Controller
    {
        private readonly IHubSenderProvider _hubSenderProvider;
        public SignalRController(IHubSenderProvider hubSenderProvider)
        {
            _hubSenderProvider = hubSenderProvider;
        }

        /// <summary>
        /// 发送消息到所有终端
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SendAll(string message)
        {
            var senders = _hubSenderProvider.GetHubSenders();
            foreach (var sender in senders) 
            {
                await sender.SendMessageToAllClientsAsync("ars.hub.ReceiveMessage", JsonConvert.SerializeObject(new { Message = message }));
            }

            return;
        }

        /// <summary>
        /// 发送消息到指定终端
        /// </summary>
        /// <param name="ternimal"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SendtoClients(string ternimal, string message)
        {
            var sender = _hubSenderProvider.GetHubSender(ternimal);
            if (null != sender)
            {
                await sender.SendMessageToAllClientsAsync("ars.hub.ReceiveMessage", message);
            }

            return;
        }

        /// <summary>
        /// 发送消息到指定客户端
        /// </summary>
        /// <param name="ternimal"></param>
        /// <param name="cid"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SendtoClient(string ternimal,string cid, string message)
        {
            var sender = _hubSenderProvider.GetHubSender(ternimal);
            if (null != sender)
            {
                await sender.SendMessageToClientAsync("ars.hub.ReceiveMessage",cid, message);
            }

            return;
        }

        /// <summary>
        /// 发送消息到指定组
        /// </summary>
        /// <param name="ternimal"></param>
        /// <param name="group"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SendtoGroup(string ternimal, string group, string message)
        {
            var sender = _hubSenderProvider.GetHubSender(ternimal);
            if (null != sender)
            {
                await sender.SendMessageToGroupAsync("ars.hub.ReceiveMessage", group, message);
            }

            return;
        }

        /// <summary>
        /// 发送消息到指定用户
        /// </summary>
        /// <param name="ternimal"></param>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task SendtoUser(string ternimal, string user, string message)
        {
            var sender = _hubSenderProvider.GetHubSender(ternimal);
            if (null != sender)
            {
                await sender.SendMessageToUserAsync("ars.hub.ReceiveMessage", user, message);
            }

            return;
        }
    }
}
