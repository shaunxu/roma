using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusReplySessionChannelListener : MessageBusChannelListenerBase<IReplySessionChannel>
    {
        public MessageBusReplySessionChannelListener(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(transportElement, context)
        {
        }

        protected override IReplySessionChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress localAddress,
            MessageBusChannelListenerBase<IReplySessionChannel> parent,
            IBus bus)
        {
            return new MessageBusReplySessionChannel(bufferManager, encoder, parent, localAddress, bus);
        }
    }
}
