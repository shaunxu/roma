using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;

namespace Roma.Bus.InProcBus
{
    public class InProcMessageBus : IBus
    {
        private readonly ConcurrentDictionary<Guid, InProcMessageEntity> _queue;
        private readonly object _lock;

        public InProcMessageBus()
        {
            _queue = new ConcurrentDictionary<Guid, InProcMessageEntity>();
            _lock = new object();
        }

        public string SendRequest(string message, string sessionId, bool fromClient, string from, string to = null)
        {
            var entity = new InProcMessageEntity(message, sessionId, fromClient, from, to);
            _queue.TryAdd(entity.ID, entity);
            return entity.ID.ToString();
        }

        public void SendReply(string message, string sessionId, bool fromClient, string replyTo)
        {
            var entity = new InProcMessageEntity(message, sessionId, fromClient, null, replyTo);
            _queue.TryAdd(entity.ID, entity);
        }

        public BusMessage Receive(bool fromClient, string replyTo)
        {
            InProcMessageEntity e = null;
            while (true)
            {
                lock (_lock)
                {
                    var entity = _queue
                        .Where(kvp => kvp.Value.FromClient == fromClient && (kvp.Value.To == replyTo || string.IsNullOrWhiteSpace(kvp.Value.To)))
                        .FirstOrDefault();
                    if (entity.Key != Guid.Empty && entity.Value != null)
                    {
                        _queue.TryRemove(entity.Key, out e);
                    }
                }
                if (e == null)
                {
                    Thread.Sleep(100);
                }
                else
                {
                    return new BusMessage(e.ID.ToString(), e.SessionID, e.From, e.To, e.Content);
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
