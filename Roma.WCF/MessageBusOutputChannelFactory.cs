using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;
using System.ServiceModel;

namespace Roma.WCF
{
    public class MessageBusOutputChannelFactory : MessageBusChannelFactoryBase<IOutputChannel>
    {
        public MessageBusOutputChannelFactory(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(transportElement, context)
        {
        }

        protected override IOutputChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress remoteAddress,
            MessageBusChannelFactoryBase<IOutputChannel> parent,
            Uri via,
            IBus bus)
        {
            return new MessageBusOutputChannel(bufferManager, encoder, parent, remoteAddress, via, bus);
        }
    }
}
