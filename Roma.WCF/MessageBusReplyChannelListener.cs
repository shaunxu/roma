using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusReplyChannelListener : MessageBusChannelListenerBase<IReplyChannel>
    {
        public MessageBusReplyChannelListener(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(transportElement, context)
        {
        }

        protected override IReplyChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress localAddress, 
            MessageBusChannelListenerBase<IReplyChannel> parent, 
            IBus bus)
        {
            return new MessageBusReplyChannel(bufferManager, encoder, parent, localAddress, bus);
        }
    }
}
