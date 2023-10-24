using Ars.Commom.Tool.Extension;
using Ars.Common.Core.AspNetCore.Extensions;
using Ars.Common.SignalR.Caches;
using Ars.Common.SignalR.Sender;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NPOI.XSSF.Streaming.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Hubs
{
    public abstract class BaseHub<T> : Hub, IHubSendMessage
        where T : Hub
    {
        public abstract string Terminal { get; }

        public abstract int Order { get; }

        protected HttpContext? _httpContext => Context.GetHttpContext();

        protected readonly IHubContext<T> _hubContext;

        protected readonly IHubCacheManager _cacheManager;

        protected readonly ILogger _logger;

        protected readonly IEnumerable<IHubDisconnection> _hubDisconnections;

        /// <summary>
        /// 心跳多少秒刷新一下缓存
        /// </summary>
        protected int HeartBeatIntervalSeconds => 15;

        private int _heartBeatIntervalSeconds = 0;

        public BaseHub(
            IHubContext<T> hubContext, IHubCacheManager cacheManager,
            ILoggerFactory loggerFactory, IEnumerable<IHubDisconnection> hubDisconnections)
        {
            _hubContext = hubContext;
            _cacheManager = cacheManager;
            _logger = loggerFactory.CreateLogger(GetType());
            _hubDisconnections = hubDisconnections;
        }

        /// <summary>
        /// 上线
        /// </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            //心跳
            await Heart();

            StringValues extension = default;
            _httpContext?.Request.Query.TryGetValue("extension", out extension);

            //添加缓存
            await _cacheManager.ClientOnConnection(Terminal, Context.ConnectionId,
                new SignalRCacheScheme 
                {
                    UserName = Context.UserIdentifier,
                    HeartTime = DateTime.Now,
                    Extension = extension.ToString()
                });

            if (_httpContext?.Request.Query.TryGetValue("group", out var value) ?? false)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, value.ToString());
            }

            await base.OnConnectedAsync();

            _logger.LogInformation("客户端:{0}连接成功,用户信息:{1}", Context.ConnectionId, Context.UserIdentifier);
        }

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            //下线通知
            if (_hubDisconnections.HasValue()) 
            {
                try
                {
                    foreach (var hub in _hubDisconnections)
                    {
                        await hub.ClientDisConnectionNoticeAysnc(
                            Terminal,
                            Context.ConnectionId,
                            await _cacheManager.GetCacheValue(Terminal, Context.ConnectionId));
                    }
                }
                catch { }
            }

            //清除缓存
            await _cacheManager.ClientDisConnection(Terminal, Context.ConnectionId);

            await base.OnDisconnectedAsync(exception);

            _logger.LogInformation("客户端:{0}下线,用户信息:{1}", Context.ConnectionId, Context.UserIdentifier);
        }

        /// <summary>
        /// 发送给所有客户端
        /// 测试一下，某个终端中的客户端，是不是包含所有的终端
        /// </summary>
        /// <param name="method"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual Task SendMessageToAllClientsAsync(string method, object? message)
        {
            return _hubContext.Clients.All.SendAsync(method, message);
        }

        /// <summary>
        /// 发送给指定客户端
        /// </summary>
        /// <param name="method"></param>
        /// <param name="connectionId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual async Task SendMessageToClientAsync(string method, string connectionId, object? message)
        {
            if (await _cacheManager.ClientIsOnline(Terminal, connectionId))
            {
                await _hubContext.Clients.Client(connectionId).SendAsync(method, message);
            }

            return;
        }

        /// <summary>
        /// 发送给指定用户
        /// </summary>
        /// <param name="method"></param>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessageToUserAsync(string method, string userName, object? message)
        {
            if (await _cacheManager.UserIsOnline(Terminal, userName))
            {
                await _hubContext.Clients.User(userName).SendAsync(method, message);
            }

            return;
        }

        /// <summary>
        /// 发送给客户组
        /// </summary>
        /// <param name="method"></param>
        /// <param name="groupName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual Task SendMessageToGroupAsync(string method, string groupName, object? message)
        {
            return _hubContext.Clients.Group(groupName).SendAsync(method, message);
        }

        /// <summary>
        /// 心跳维持
        /// </summary>
        private Task Heart()
        {
            var feature = _httpContext?.Features.Get<IConnectionHeartbeatFeature>();

            string connectionid = Context.ConnectionId;
            //默认对连接的心跳扫描为1s触发1次
            feature?.OnHeartbeat(async obj =>
            {
                int time = Interlocked.Increment(ref _heartBeatIntervalSeconds);

                //每15s刷新一下过期时间
                if (time % HeartBeatIntervalSeconds == 0)
                {
                    await _cacheManager.Refresh(Terminal, connectionid);
                }
            }, _httpContext!);

            return Task.CompletedTask;
        }
    }
}
