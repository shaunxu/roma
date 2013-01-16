using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusDuplexSessionChannelFactory : MessageBusChannelFactoryBase<IDuplexSessionChannel>
    {
        public MessageBusDuplexSessionChannelFactory(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(transportElement, context)
        {
        }

        protected override IDuplexSessionChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress remoteAddress,
            MessageBusChannelFactoryBase<IDuplexSessionChannel> parent,
            Uri via,
            IBus bus)
        {
            return new MessageBusDuplexSessionChannel(bufferManager, encoder, parent, remoteAddress, via, bus, true);
        }
    }
}
