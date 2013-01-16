using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;
using System.ServiceModel;
using System.Xml;

namespace Roma.WCF
{
    public class MessageBusOutputSessionChannel : MessageBusOutputChannel, IOutputSessionChannel
    {
        private IOutputSession _session;
        private readonly IBus _bus;

        public MessageBusOutputSessionChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, ChannelManagerBase parent,
            EndpointAddress remoteAddress, Uri via,
            IBus bus)
            : base(bufferManager, encoder, parent, remoteAddress, via, bus)
        {
            _bus = bus;
            _session = new MessageBusOutputSession((new UniqueId()).ToString());
        }

        public IOutputSession Session
        {
            get
            {
                return _session;
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
            base.OnOpen(timeout);
        }

        public override void Send(Message message, TimeSpan timeout)
        {
            // add the message id if not
            var messageId = new System.Xml.UniqueId();
            if (message.Headers.MessageId == null)
            {
                message.Headers.MessageId = messageId;
            }
            // send message with session id
            var content = GetStringFromWcfMessage(message, RemoteAddress);
            _bus.SendRequest(content, _session.Id, true, ChannelID, null);
            // wait for the acknowledge message from the server side
            _bus.Receive(false, messageId.ToString());
        }
    }
}
