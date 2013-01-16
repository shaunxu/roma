using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusTransportBindingElement : TransportBindingElement
    {
        public const string CST_SCHEME = "net.bus";

        private readonly IBus _bus;

        public IBus Bus
        {
            get
            {
                return _bus;
            }
        }

        public override string Scheme
        {
            get
            {
                return CST_SCHEME;
            }
        }

        public MessageBusTransportBindingElement(IBus bus)
            : base()
        {
            _bus = bus;
        }

        public MessageBusTransportBindingElement(MessageBusTransportBindingElement other)
            : base(other)
        {
            _bus = other._bus;
        }

        public override BindingElement Clone()
        {
            return new MessageBusTransportBindingElement(this);
        }

        public override bool CanBuildChannelFactory<TChannel>(BindingContext context)
        {
            return typeof(TChannel) == typeof(IRequestChannel) ||
                   typeof(TChannel) == typeof(IOutputChannel) ||
                   typeof(TChannel) == typeof(IDuplexChannel) ||
                   typeof(TChannel) == typeof(IRequestSessionChannel) ||
                   typeof(TChannel) == typeof(IOutputSessionChannel) ||
                   typeof(TChannel) == typeof(IDuplexSessionChannel);
        }

        public override bool CanBuildChannelListener<TChannel>(BindingContext context)
        {
            return typeof(TChannel) == typeof(IReplyChannel) ||
                   typeof(TChannel) == typeof(IInputChannel) ||
                   typeof(TChannel) == typeof(IDuplexChannel) ||
                   typeof(TChannel) == typeof(IReplySessionChannel) ||
                   typeof(TChannel) == typeof(IInputSessionChannel) ||
                   typeof(TChannel) == typeof(IDuplexSessionChannel);
        }

        public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (!CanBuildChannelFactory<TChannel>(context))
            {
                throw new ArgumentException(string.Format("Unsupported channel type: {0} for the channel factory.", typeof(TChannel).Name));
            }

            if (typeof(TChannel) == typeof(IRequestChannel))
            {
                return (IChannelFactory<TChannel>)(object)new MessageBusRequestChannelFactory(this, context);
            }
            else if (typeof(TChannel) == typeof(IOutputChannel))
            {
                return (IChannelFactory<TChannel>)(object)new MessageBusOutputChannelFactory(this, context);
            }
            else if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return (IChannelFactory<TChannel>)(object)new MessageBusDuplexChannelFactory(this, context);
            }
            else if (typeof(TChannel) == typeof(IRequestSessionChannel))
            {
                return (IChannelFactory<TChannel>)(object)new MessageBusRequestSessionChannelFactory(this, context);
            }
            else if (typeof(TChannel) == typeof(IOutputSessionChannel))
            {
                return (IChannelFactory<TChannel>)(object)new MessageBusOutputSessionChannelFactory(this, context);
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (IChannelFactory<TChannel>)(object)new MessageBusDuplexSessionChannelFactory(this, context);
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported channel type: {0} for the channel listener.", typeof(TChannel).Name));
            }

        }

        public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (!CanBuildChannelListener<TChannel>(context))
            {
                throw new ArgumentException(string.Format("Unsupported channel type: {0} for the channel listener.", typeof(TChannel).Name));
            }

            if (typeof(TChannel) == typeof(IReplyChannel))
            {
                return (IChannelListener<TChannel>)(object)new MessageBusReplyChannelListener(this, context);
            }
            else if (typeof(TChannel) == typeof(IInputChannel))
            {
                return (IChannelListener<TChannel>)(object)new MessageBusInputChannelListener(this, context);
            }
            else if (typeof(TChannel) == typeof(IDuplexChannel))
            {
                return (IChannelListener<TChannel>)(object)new MessageBusDuplexChannelListener(this, context);
            }
            else if (typeof(TChannel) == typeof(IReplySessionChannel))
            {
                return (IChannelListener<TChannel>)(object)new MessageBusReplySessionChannelListener(this, context);
            }
            else if (typeof(TChannel) == typeof(IInputSessionChannel))
            {
                return (IChannelListener<TChannel>)(object)new MessageBusInputSessionChannelListener(this, context);
            }
            else if (typeof(TChannel) == typeof(IDuplexSessionChannel))
            {
                return (IChannelListener<TChannel>)(object)new MessageBusDuplexSessionChannelListener(this, context);
            }
            else
            {
                throw new ArgumentException(string.Format("Unsupported channel type: {0} for the channel listener.", typeof(TChannel).Name));
            }
        }
    }
}
