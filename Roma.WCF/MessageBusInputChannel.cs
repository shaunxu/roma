using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;
using System.ServiceModel;

namespace Roma.WCF
{
    public class MessageBusInputChannel : MessageBusChannelBase, IInputChannel
    {
        private readonly IBus _bus;
        private readonly EndpointAddress _localAddress;
        private readonly object _aLock;

        private delegate bool TryReceiveDelegate(TimeSpan timeout, out Message message);
        private TryReceiveDelegate _tryReceiveDelegate;

        private delegate Message ReceiveDelegate(TimeSpan timeout);
        private ReceiveDelegate _receiveDelegate;

        public MessageBusInputChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, ChannelManagerBase parent,
            EndpointAddress localAddress,
            IBus bus)
            : base(bufferManager, encoder, parent)
        {
            _localAddress = localAddress;
            _bus = bus;
            _aLock = new object();

            _tryReceiveDelegate = (TimeSpan timeout, out Message message) =>
            {
                message = null;
                try
                {
                    var requestMessage = _bus.Receive(true, null);
                    if (requestMessage != null)
                    {
                        message = GetWcfMessageFromString(requestMessage.Content);
                        OnAfterTryReceive(requestMessage);
                    }
                }
                catch (Exception ex)
                {
                    throw new CommunicationException(ex.Message, ex);
                }
                return true;
            };

            _receiveDelegate = (TimeSpan timeout) =>
            {
                var requestMessage = _bus.Receive(false, ChannelID);
                return GetWcfMessageFromString(requestMessage.Content);
            };
        }

        public System.ServiceModel.EndpointAddress LocalAddress
        {
            get
            {
                return _localAddress;
            }
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            return _receiveDelegate.BeginInvoke(timeout, callback, state);
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            return BeginReceive(DefaultReceiveTimeout, callback, state);
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            Message message;
            return _tryReceiveDelegate.BeginInvoke(timeout, out message, callback, state);
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public Message EndReceive(IAsyncResult result)
        {
            return _receiveDelegate.EndInvoke(result);
        }

        public virtual bool EndTryReceive(IAsyncResult result, out Message message)
        {
            var ret = _tryReceiveDelegate.EndInvoke(out message, result);
            return ret;
        }

        protected virtual void OnAfterTryReceive(BusMessage message)
        {
        }

        public bool EndWaitForMessage(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public Message Receive(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Message Receive()
        {
            throw new NotImplementedException();
        }

        public bool TryReceive(TimeSpan timeout, out Message message)
        {
            throw new NotImplementedException();
        }

        public bool WaitForMessage(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }
    }
}
