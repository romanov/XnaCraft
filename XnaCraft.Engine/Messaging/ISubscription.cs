using System;

namespace XnaCraft.Engine.Messaging
{
    public interface ISubscription : IDisposable
    {
        void Cancel();
    }
}