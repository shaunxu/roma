using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;
using System.ServiceModel;

namespace Roma.WCF
{
    public class MessageBusRequestChannel : MessageBusChannelBase, IRequestChannel
    {
        private readonly IBus _bus;
        private readonly Uri _via;
        private readonly EndpointAddress _remoteAddress;
        private readonly object _aLock;

        public MessageBusRequestChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, ChannelManagerBase parent,
            EndpointAddress remoteAddress, Uri via, IBus bus)
            : base(bufferManager, encoder, parent)
        {
            _via = via;
            _remoteAddress = remoteAddress;
            _bus = bus;
            _aLock = new object();
        }

        public Uri Via
        {
            get
            {
                return _via;
            }
        }

        public IAsyncResult BeginRequest(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRequest(Message message, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public Message EndRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public System.ServiceModel.EndpointAddress RemoteAddress
        {
            get 
            {
                return _remoteAddress;
            }
        }

        public Message Request(Message message, TimeSpan timeout)
        {
            ThrowIfDisposedOrNotOpen();
            lock (_aLock)
            {
                // unbox the message into string that will be sent into the bus
                var content = GetStringFromWcfMessage(message,_remoteAddress);
                // apply the session from the sub class if needed
                var sessionId = string.Empty;
                OnBeforeRequest(ref sessionId);
                // send the message into bus
                var busMsgId = _bus.SendRequest(content, sessionId, true, null);
                // waiting for the reply message arrive from the bus
                var replyMsg = _bus.Receive(false, busMsgId);
                if (string.IsNullOrWhiteSpace(replyMsg.Content))
                {
                    // this means this is a one way channel acknowledge from server
                    // we just return null and do nothing
                    return null;
                }
                else
                {
                    // box the message from the bus message content and return back
                    var reply = GetWcfMessageFromString(replyMsg.Content);
                    return reply;
                }
            }
        }

        protected virtual void OnBeforeRequest(ref string sessionId)
        {
        }

        public Message Request(Message message)
        {
            return Request(message, DefaultSendTimeout);
        }
    }
}
