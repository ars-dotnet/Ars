using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Sender
{
    /// <summary>
    /// 服务端发送消息
    /// </summary>
    public interface IHubSendMessage
    {
        /// <summary>
        /// 终端名称相同时
        /// order升序找到第一个实现的hub
        /// </summary>
        int Order { get; }

        /// <summary>
        /// 终端名称
        /// web/android/ios/wap
        /// </summary>
        string Terminal { get; }

        /// <summary>
        /// 发送到所有客户端
        /// </summary>
        /// <param name="method"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessageToAllClientsAsync(string method, string message);

        /// <summary>
        /// 发送指定客户端
        /// </summary>
        /// <param name="method"></param>
        /// <param name="connectionId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessageToClientAsync(string method, string connectionId, string message);

        /// <summary>
        /// 发送指定用户
        /// </summary>
        /// <param name="method"></param>
        /// <param name="userId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessageToUserAsync(string method, string userId, string message);

        /// <summary>
        /// 发送指定组
        /// </summary>
        /// <param name="method"></param>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task SendMessageToGroupAsync(string method, string groupName, string message);
    }
}
