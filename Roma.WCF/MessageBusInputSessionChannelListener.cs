﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusInputSessionChannelListener : MessageBusChannelListenerBase<IInputSessionChannel>
    {
        public MessageBusInputSessionChannelListener(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(transportElement, context)
        {
        }

        protected override IInputSessionChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress localAddress,
            MessageBusChannelListenerBase<IInputSessionChannel> parent, 
            IBus bus)
        {
            return new MessageBusInputSessionChannel(bufferManager, encoder, parent, localAddress, bus);
        }
    }
}
