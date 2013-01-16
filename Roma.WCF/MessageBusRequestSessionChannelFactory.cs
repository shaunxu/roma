using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusRequestSessionChannelFactory : MessageBusChannelFactoryBase<IRequestSessionChannel>
    {
        public MessageBusRequestSessionChannelFactory(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(transportElement, context)
        {
        }

        protected override IRequestSessionChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress remoteAddress,
            MessageBusChannelFactoryBase<IRequestSessionChannel> parent, 
            Uri via, 
            IBus bus)
        {
            return new MessageBusRequestSessionChannel(bufferManager, encoder, parent, remoteAddress, via, bus);
        }
    }
}
