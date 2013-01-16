using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusReplySessionChannel : MessageBusReplyChannel, IReplySessionChannel
    {
        private IInputSession _session;

        public IInputSession Session
        {
            get
            {
                return _session;
            }
        }

        public MessageBusReplySessionChannel(
            BufferManager bufferManager, MessageEncoderFactory encoderFactory, ChannelManagerBase parent,
            EndpointAddress localAddress,
            IBus bus)
            : base(bufferManager, encoderFactory, parent, localAddress, bus)
        {
        }

        protected override void OnAfterTryReceiveRequest(BusMessage message)
        {
            _session = new MessageBusInputSession(message.SessionID);
        }
    }
}
