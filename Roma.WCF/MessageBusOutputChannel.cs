using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;
using System.ServiceModel;

namespace Roma.WCF
{
    public class MessageBusOutputChannel : MessageBusChannelBase, IOutputChannel
    {
        private readonly IBus _bus;
        private readonly Uri _via;
        private readonly EndpointAddress _remoteAddress;

        public MessageBusOutputChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, ChannelManagerBase parent,
            EndpointAddress remoteAddress,
            Uri via, IBus bus)
            : base(bufferManager, encoder, parent)
        {
            _bus = bus;
            _via = via;
            _remoteAddress = remoteAddress;
        }

        public Uri Via
        {
            get
            {
                return _via;
            }
        }

        public EndpointAddress RemoteAddress
        {
            get
            {
                return _remoteAddress;
            }
        }

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void EndSend(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public virtual void Send(Message message, TimeSpan timeout)
        {
            var content = GetStringFromWcfMessage(message, RemoteAddress);
            // apply the session from the sub class if needed
            var sessionId = string.Empty;
            OnBeforeSend(ref sessionId);
            _bus.SendRequest(content, sessionId, true, ChannelID, null);
        }

        protected virtual void OnBeforeSend(ref string sessionId)
        {
        }

        public void Send(Message message)
        {
            Send(message, DefaultSendTimeout);
        }
    }
}
