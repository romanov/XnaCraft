using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XnaCraft.Engine.Messaging
{
    public class EventManager : IEventManager
    {
        private readonly Dictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();

        public ISubscription Subscribe<TEvent>(Action<TEvent> handler) where TEvent : IEvent
        {
            var eventType = typeof (TEvent);

            List<object> eventHandlers;

            if (!_handlers.TryGetValue(eventType, out eventHandlers))
            {
                eventHandlers = new List<object>();

                _handlers.Add(eventType, eventHandlers);
            }

            eventHandlers.Add(handler);

            return new Subscription(() => eventHandlers.Remove(handler));
        }

        public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var eventType = typeof (TEvent);

            List<object> eventHandlers;

            if (_handlers.TryGetValue(eventType, out eventHandlers))
            {
                foreach (Action<TEvent> eventHandler in eventHandlers)
                {
                    eventHandler(@event);
                }
            }
        }

        private class Subscription : ISubscription
        {
            private readonly Action _cancellator;

            public Subscription(Action cancellator)
            {
                _cancellator = cancellator;
            }

            public void Cancel()
            {
                _cancellator();
            }

            public void Dispose()
            {
                Cancel();
            }
        }
    }
}
