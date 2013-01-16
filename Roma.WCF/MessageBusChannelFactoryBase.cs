using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;
using System.ServiceModel;

namespace Roma.WCF
{
    public abstract class MessageBusChannelFactoryBase<TChannel> : ChannelFactoryBase<TChannel> where TChannel : class, IChannel
    {
        private readonly MessageBusTransportBindingElement _transportElement;
        private readonly BufferManager _bufferManager;
        private readonly MessageEncoderFactory _encoder;

        private readonly IBus _bus;

        public IBus Bus
        {
            get
            {
                return _bus;
            }
        }

        protected MessageBusTransportBindingElement TransportElement
        {
            get
            {
                return _transportElement;
            }
        }

        protected MessageBusChannelFactoryBase(MessageBusTransportBindingElement transportElement, BindingContext context)
            : base(context.Binding)
        {
            _transportElement = transportElement;
            _bufferManager = BufferManager.CreateBufferManager(transportElement.MaxBufferPoolSize, int.MaxValue);
            var encodingElement = context.Binding.Elements.Find<MessageEncodingBindingElement>();
            if (encodingElement == null)
            {
                _encoder = (new TextMessageEncodingBindingElement()).CreateMessageEncoderFactory();
            }
            else
            {
                _encoder = encodingElement.CreateMessageEncoderFactory();
            }
            _bus = transportElement.Bus;
        }

        protected abstract TChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress remoteAddress,
            MessageBusChannelFactoryBase<TChannel> parent,
            Uri via,
            IBus bus);

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override void OnClosed()
        {
            base.OnClosed();

            _bufferManager.Clear();
            _bus.Dispose();
        }

        protected override TChannel OnCreateChannel(System.ServiceModel.EndpointAddress address, Uri via)
        {
            return CreateChannel(_bufferManager, _encoder, address, this, via, _bus);
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }
}
