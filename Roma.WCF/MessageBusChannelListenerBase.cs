using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;
using System.ServiceModel;

namespace Roma.WCF
{
    public abstract class MessageBusChannelListenerBase<TChannel> : ChannelListenerBase<TChannel> where TChannel : class, IChannel
    {
        private readonly MessageBusTransportBindingElement _transportElement;

        private readonly BufferManager _bufferManager;
        private readonly MessageEncoderFactory _encoder;
        private readonly string _scheme;
        private readonly Uri _uri;

        private readonly IBus _bus;

        private readonly InputQueue<TChannel> _channelQueue;
        private readonly object _currentChannelLock;

        private TChannel _currentChannel;

        public IBus Bus
        {
            get
            {
                return _bus;
            }
        }

        public override Uri Uri
        {
            get
            {
                return _uri;
            }
        }

        public string Scheme
        {
            get
            {
                return _scheme;
            }
        }

        protected MessageBusTransportBindingElement TransportElement
        {
            get
            {
                return _transportElement;
            }
        }

        protected MessageBusChannelListenerBase(MessageBusTransportBindingElement transportElement, BindingContext context)
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
            _scheme = transportElement.Scheme;
            _uri = new Uri(context.ListenUriBaseAddress, context.ListenUriRelativeAddress);

            _bus = transportElement.Bus;

            _channelQueue = new InputQueue<TChannel>();
            _currentChannelLock = new object();
            _currentChannel = null;
        }

        protected override void OnAbort()
        {
            try
            {
                lock (ThisLock)
                {
                    _channelQueue.Close();
                }
            }
            catch { }
        }

        protected override void OnClose(TimeSpan timeout)
        {
            try
            {
                lock (ThisLock)
                {
                    _channelQueue.Close();
                }
            }
            catch { }
        }

        protected override void OnClosed()
        {
            base.OnClosed();

            try
            {
                _bufferManager.Clear();
                _bus.Dispose();
            }
            catch { }
        }

        private void EnsureChannelAvailable()
        {
            TChannel newChannel = null;
            bool channelCreated = false;

            if ((newChannel = _currentChannel) == null)
            {
                lock (_currentChannelLock)
                {
                    if ((newChannel = _currentChannel) == null)
                    {
                        newChannel = CreateChannel(_bufferManager, _encoder, new EndpointAddress(_uri), this, _bus);
                        newChannel.Closed += new EventHandler(OnChannelClosed);
                        _currentChannel = newChannel;
                        channelCreated = true;
                    }
                }
            }

            if (channelCreated)
            {
                _channelQueue.EnqueueAndDispatch(newChannel);
            }
        }

        protected abstract TChannel CreateChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress localAddress,
            MessageBusChannelListenerBase<TChannel> parent,
            IBus bus);

        private void OnChannelClosed(object sender, EventArgs e)
        {
            var channel = sender as TChannel;
            lock (_currentChannelLock)
            {
                if (channel == _currentChannel)
                {
                    _currentChannel = null;
                }
            }
        }

        protected override TChannel OnAcceptChannel(TimeSpan timeout)
        {
            if (!IsDisposed)
            {
                EnsureChannelAvailable();
            }

            TChannel channel = null;
            if (_channelQueue.Dequeue(timeout, out channel))
            {
                return channel;
            }
            else
            {
                throw new TimeoutException();
            }
        }

        protected override IAsyncResult OnBeginAcceptChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            if (!IsDisposed)
            {
                EnsureChannelAvailable();
            }

            return _channelQueue.BeginDequeue(timeout, callback, state);
        }

        protected override TChannel OnEndAcceptChannel(IAsyncResult result)
        {
            TChannel channel;
            if (_channelQueue.EndDequeue(result, out channel))
            {
                return channel;
            }
            else
            {
                throw new TimeoutException();
            }
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        protected override IAsyncResult OnBeginWaitForChannel(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override bool OnEndWaitForChannel(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        protected override bool OnWaitForChannel(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        protected override IAsyncResult OnBeginClose(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override IAsyncResult OnBeginOpen(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        protected override void OnEndClose(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        protected override void OnEndOpen(IAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }
}
