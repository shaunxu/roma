using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusDuplexChannelFactory : MessageBusChannelFactoryBase<IDuplexChannel>
    {
        public MessageBusDuplexChannelFactory(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(transportElement, context)
        {
        }

        protected override IDuplexChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress remoteAddress,
            MessageBusChannelFactoryBase<IDuplexChannel> parent,
            Uri via,
            IBus bus)
        {
            return new MessageBusDuplexChannel(bufferManager, encoder, remoteAddress, parent, via, bus, true);
        }
    }
}
