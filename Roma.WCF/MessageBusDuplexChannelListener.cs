using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusDuplexChannelListener : MessageBusChannelListenerBase<IDuplexChannel>
    {
        public MessageBusDuplexChannelListener(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(transportElement, context)
        {
        }

        protected override IDuplexChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress localAddress,
            MessageBusChannelListenerBase<IDuplexChannel> parent, 
            IBus bus)
        {
            return new MessageBusDuplexChannel(bufferManager, encoder, localAddress, parent, null, bus, false);
        }
    }
}
