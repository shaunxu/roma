using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;
using System.ServiceModel;

namespace Roma.WCF
{
    public class MessageBusInputSessionChannel : MessageBusInputChannel, IInputSessionChannel
    {
        private IInputSession _session;
        private readonly IBus _bus;

        public IInputSession Session
        {
            get
            {
                return _session;
            }
        }

        public MessageBusInputSessionChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, ChannelManagerBase parent,
            EndpointAddress localAddress,
            IBus bus)
            : base(bufferManager, encoder, parent, localAddress, bus)
        {
            _bus = bus;
        }

        protected override void OnAfterTryReceive(BusMessage message)
        {
            _session = new MessageBusInputSession(message.SessionID);
        }

        public override bool EndTryReceive(IAsyncResult result, out Message message)
        {
            var ret = base.EndTryReceive(result, out message);
            // unbox the message id and send the acknowledge message back to the client
            var messageId = message.Headers.MessageId;
            _bus.SendReply(string.Empty, _session.Id, false, messageId.ToString());
            return ret;
        }
    }
}
