using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine.Messaging
{
    public interface IEventManager
    {
        void Publish<TEvent>(TEvent @event) where TEvent: IEvent;
        ISubscription Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent;
    }
}
