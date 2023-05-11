using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ars.Common.SignalR.Sender
{
    public interface IHubSenderProvider
    {
        IHubSendMessage? GetHubSender(string terminal);

        IEnumerable<IHubSendMessage> GetHubSenders();
    }
}
