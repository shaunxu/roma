using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusDuplexSessionChannelListener : MessageBusChannelListenerBase<IDuplexSessionChannel>
    {
        public MessageBusDuplexSessionChannelListener(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(transportElement, context)
        {
        }

        protected override IDuplexSessionChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress localAddress,
            MessageBusChannelListenerBase<IDuplexSessionChannel> parent, 
            IBus bus)
        {
            return new MessageBusDuplexSessionChannel(bufferManager, encoder, parent, localAddress, null, bus, false);
        }
    }
}
