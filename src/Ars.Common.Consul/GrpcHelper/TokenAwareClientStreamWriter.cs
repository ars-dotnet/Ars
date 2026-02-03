using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.Consul.GrpcHelper
{
    /// <summary>
    /// 支持 Token 感知的 ClientStreamWriter 包装器
    /// </summary>
    public class TokenAwareClientStreamWriter<T> : IClientStreamWriter<T>
    {
        private readonly IClientStreamWriter<T> _innerWriter;
        private readonly Func<T, Task>? _beforeWriteAction;

        public TokenAwareClientStreamWriter(
            IClientStreamWriter<T> innerWriter,
            Func<T, Task>? beforeWriteAction = null)
        {
            _innerWriter = innerWriter;
            _beforeWriteAction = beforeWriteAction;
        }

        public WriteOptions? WriteOptions
        {
            get => _innerWriter.WriteOptions;
            set => _innerWriter.WriteOptions = value;
        }

        public async Task WriteAsync(T message)
        {
            if (_beforeWriteAction != null)
            {
                await _beforeWriteAction(message);
            }

            await _innerWriter.WriteAsync(message);
        }

        public async Task CompleteAsync()
        {
            if (_beforeWriteAction != null)
            {
                await _beforeWriteAction(default);
            }

            await _innerWriter.CompleteAsync();
        }
    }
}
