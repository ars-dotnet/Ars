using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR
{
    public class SignalRCacheScheme
    {
        /// <summary>
        /// 心跳时间
        /// </summary>
        public DateTime HeartTime { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// 扩展信息
        /// </summary>
        public string? Extension { get; set; }
    }
}
