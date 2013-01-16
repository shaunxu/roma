using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusOutputSessionChannelFactory : MessageBusChannelFactoryBase<IOutputSessionChannel>
    {
        public MessageBusOutputSessionChannelFactory(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(transportElement, context)
        {
        }

        protected override IOutputSessionChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress remoteAddress,
            MessageBusChannelFactoryBase<IOutputSessionChannel> parent,
            Uri via,
            IBus bus)
        {
            return new MessageBusOutputSessionChannel(bufferManager, encoder, parent, remoteAddress, via, bus);
        }
    }
}
