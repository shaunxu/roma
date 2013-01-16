using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusInputChannelListener : MessageBusChannelListenerBase<IInputChannel>
    {
        public MessageBusInputChannelListener(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(transportElement, context)
        {
        }

        protected override IInputChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress localAddress,
            MessageBusChannelListenerBase<IInputChannel> parent, 
            IBus bus)
        {
            return new MessageBusInputChannel(bufferManager, encoder, parent, localAddress, bus);
        }
    }
}
