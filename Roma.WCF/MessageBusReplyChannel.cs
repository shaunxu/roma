using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;
using System.ServiceModel;

namespace Roma.WCF
{
    public class MessageBusReplyChannel : MessageBusChannelBase, IReplyChannel
    {
        private readonly EndpointAddress _localAddress;
        private readonly object _aLock;

        private readonly IBus _bus;

        private delegate bool TryReceiveRequestDelegate(TimeSpan timeout, out RequestContext context);
        private TryReceiveRequestDelegate _tryReceiveRequestDelegate;

        public MessageBusReplyChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, ChannelManagerBase parent,
            EndpointAddress localAddress,
            IBus bus)
            : base(bufferManager, encoder, parent)
        {
            _localAddress = localAddress;
            _bus = bus;
            _aLock = new object();

            _tryReceiveRequestDelegate = (TimeSpan t, out RequestContext rc) =>
            {
                rc = null;
                // receive the request message from the bus
                var busMsg = _bus.Receive(true, null);
                // box the wcf message
                var message = GetWcfMessageFromString(busMsg.Content);
                // initialize the request context and return
                rc = new MessageBusRequestContext(message, this, _localAddress, _bus, busMsg.MessageID);
                OnAfterTryReceiveRequest(busMsg);
                return true;
            };
        }

        protected virtual void OnAfterTryReceiveRequest(BusMessage message)
        {
        }

        public System.ServiceModel.EndpointAddress LocalAddress
        {
            get
            {
                return _localAddress;
            }
        }

        public bool WaitForRequest(TimeSpan timeout)
        {
            return true;
        }

        public RequestContext ReceiveRequest()
        {
            return ReceiveRequest(DefaultReceiveTimeout);
        }

        public RequestContext ReceiveRequest(TimeSpan timeout)
        {
            ThrowIfDisposedOrNotOpen();
            lock (_aLock)
            {
                // receive the request message from the bus
                var busMsg = _bus.Receive(true, null);
                // box the wcf message
                var message = GetWcfMessageFromString(busMsg.Content);
                // initialize the request context and return
                return new MessageBusRequestContext(message, this, _localAddress, _bus, busMsg.MessageID);
            }
        }

        public IAsyncResult BeginTryReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            RequestContext context;
            return _tryReceiveRequestDelegate.BeginInvoke(timeout, out context, callback, state);
        }

        public bool EndTryReceiveRequest(IAsyncResult result, out RequestContext context)
        {
            var ret = _tryReceiveRequestDelegate.EndInvoke(out context, result);
            return ret;
        }

        public IAsyncResult BeginReceiveRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginReceiveRequest(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginWaitForRequest(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public RequestContext EndReceiveRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public bool EndWaitForRequest(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public bool TryReceiveRequest(TimeSpan timeout, out RequestContext context)
        {
            throw new NotImplementedException();
        }
    }
}
