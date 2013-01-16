using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;
using System.ServiceModel;

namespace Roma.WCF
{
    public class MessageBusRequestChannelFactory : MessageBusChannelFactoryBase<IRequestChannel>
    {
        public MessageBusRequestChannelFactory(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(transportElement, context)
        {
        }

        protected override IRequestChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress remoteAddress, 
            MessageBusChannelFactoryBase<IRequestChannel> parent, 
            Uri via, 
            IBus bus)
        {
            return new MessageBusRequestChannel(bufferManager, encoder, parent, remoteAddress, via, bus);
        }
    }
}
