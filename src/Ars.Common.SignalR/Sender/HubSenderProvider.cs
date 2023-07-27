using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Sender
{
    internal class HubSenderProvider : IHubSenderProvider
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public HubSenderProvider(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public IHubSendMessage? GetHubSender(string terminal)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return scope.ServiceProvider.GetService<IEnumerable<IHubSendMessage>>()?.OrderBy(r => r.Order)?.FirstOrDefault(r => r.Terminal.Equals(terminal));
        }

        public IEnumerable<IHubSendMessage> GetHubSenders()
        {
            using var scope = _serviceScopeFactory.CreateScope();
            return scope.ServiceProvider.GetService<IEnumerable<IHubSendMessage>>() ?? Array.Empty<IHubSendMessage>();
        }
    }
}
