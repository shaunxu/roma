using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusRequestContext : RequestContext
    {
        private bool _aborted;
        private readonly Message _message;
        private readonly MessageBusReplyChannel _parent;
        private readonly EndpointAddress _address;
        private readonly object _aLock;
        private readonly string _busMessageId;
        private readonly IBus _bus;

        private CommunicationState _state;

        public MessageBusRequestContext(
            Message message, MessageBusReplyChannel parent,
            EndpointAddress address,
            IBus bus,
            string relatedTo)
        {
            _aborted = false;
            _parent = parent;
            _message = message;
            _address = address;
            _busMessageId = relatedTo;
            _bus = bus;

            _aLock = new object();
            _state = CommunicationState.Opened;
        }

        public override void Abort()
        {
            lock (_aLock)
            {
                if (_aborted)
                {
                    return;
                }
                _aborted = true;
                _state = CommunicationState.Faulted;
            }
        }

        public override IAsyncResult BeginReply(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public override IAsyncResult BeginReply(Message message, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public override void Close(TimeSpan timeout)
        {
            lock (_aLock)
            {
                _state = CommunicationState.Closed;
            }
        }

        public override void Close()
        {
            Close(TimeSpan.MaxValue);
        }

        public override void EndReply(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnBeforeReply(ref string sessionId)
        {
        }

        public override void Reply(Message message, TimeSpan timeout)
        {
            // apply the session from the sub class if needed
            var sessionId = string.Empty;
            OnBeforeReply(ref sessionId);
            if (message == null)
            {
                // this means this is a one way message
                // we just need to send a blank message back to the client to acknowledge it
                _bus.SendReply(string.Empty, sessionId, false, _busMessageId);
            }
            else
            {
                // unbox the reply message to string
                var content = _parent.GetStringFromWcfMessage(message, _address);
                // send the reply into bus
                _bus.SendReply(content, sessionId, false, _busMessageId);
            }
        }

        public override void Reply(Message message)
        {
            Reply(message, TimeSpan.MaxValue);
        }

        public override Message RequestMessage
        {
            get
            {
                return _message;
            }
        }
    }
}
