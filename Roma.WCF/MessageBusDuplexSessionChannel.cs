using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;
using System.Xml;

namespace Roma.WCF
{
    public class MessageBusDuplexSessionChannel : MessageBusDuplexChannel, IDuplexSessionChannel
    {
        private IDuplexSession _session;

        public IDuplexSession Session
        {
            get
            {
                return _session;
            }
        }

        public MessageBusDuplexSessionChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, ChannelManagerBase parent,
            EndpointAddress remoteAddress, Uri via,
            IBus bus,
            bool isClient)
            : base(bufferManager, encoder, remoteAddress, parent, via, bus, isClient)
        {
            _session = new MessageBusDuplexSession((new UniqueId()).ToString());
        }

        protected override void OnAfterTryReceive(BusMessage message)
        {
            _session = new MessageBusDuplexSession(message.SessionID);
        }

        protected override void OnBeforeSend(ref string sessionId)
        {
            sessionId = _session.Id;
        }
    }
}
