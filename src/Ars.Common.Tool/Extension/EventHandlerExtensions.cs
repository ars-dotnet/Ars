using System;
using System.Collections.Generic;
using System.Text;

namespace Ars.Common.Tool.Extension
{
    public static class EventHandlerExtensions
    {
        public static void InvokeSafely(this EventHandler eventHandler,object sender) 
        {
            eventHandler?.Invoke(sender, EventArgs.Empty);
        }

        public static void InvokeSafely<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs eventArgs)
            where TEventArgs : EventArgs
        {
            eventHandler?.Invoke(sender, eventArgs);
        }
    }
}
