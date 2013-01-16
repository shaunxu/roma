using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;
using System.ServiceModel;
using System.Xml;
using System.Collections.Concurrent;

namespace Roma.WCF
{
    public class MessageBusDuplexChannel : MessageBusChannelBase, IDuplexChannel
    {
        private readonly IBus _bus;
        private readonly Uri _via;
        private readonly EndpointAddress _serverAddress;
        private readonly ConcurrentDictionary<UniqueId, string> _replyTos;
        private readonly bool _isClient;

        private delegate bool TryReceiveDelegate(TimeSpan timeout, out Message message);
        private readonly TryReceiveDelegate _tryReceiveDelegate;

        public MessageBusDuplexChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, EndpointAddress remoteAddress,
            ChannelManagerBase parent, Uri via,
            IBus bus, bool isClient)
            : base(bufferManager, encoder, parent)
        {
            _serverAddress = remoteAddress;
            _via = via;
            _bus = bus;
            _isClient = isClient;
            _replyTos = new ConcurrentDictionary<UniqueId, string>();

            _tryReceiveDelegate = (TimeSpan timeout, out Message message) =>
            {
                message = null;
                try
                {
                    // listen the message bus based on the sticky mode: 
                    // channel: only receive the message that reply to this channel's id
                    // scaling gourp: receive the message the reply to this channel's id and the scaling group id of this channel
                    var requestMessage = _bus.Receive(!_isClient, ChannelID);
                    if (requestMessage != null)
                    {
                        message = GetWcfMessageFromString(requestMessage.Content);
                        if (message.Headers.MessageId != null)
                        {
                            _replyTos.AddOrUpdate(message.Headers.MessageId, requestMessage.From, (key, value) => requestMessage.From);
                        }
                        OnAfterTryReceive(requestMessage);
                    }
                }
                catch (Exception ex)
                {
                    throw new CommunicationException(ex.Message, ex);
                }
                return true;
            };
        }

        public EndpointAddress LocalAddress
        {
            get 
            {
                if (_isClient)
                {
                    return new EndpointAddress(EndpointAddress.AnonymousUri);
                }
                else
                {
                    return _serverAddress;
                }
            }
        }

        public EndpointAddress RemoteAddress
        {
            get 
            {
                if (_isClient)
                {
                    return _serverAddress;
                }
                else
                {
                    return new EndpointAddress(EndpointAddress.AnonymousUri);
                }
            }
        }

        public Uri Via
        {
            get 
            {
                return _via;
            }
        }

        protected virtual void OnAfterTryReceive(BusMessage message)
        {
        }

        public IAsyncResult BeginTryReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            Message message;
            return _tryReceiveDelegate.BeginInvoke(timeout, out message, callback, state);
        }

        public bool EndTryReceive(IAsyncResult result, out Message message)
        {
            var ret = _tryReceiveDelegate.EndInvoke(out message, result);
            return ret;
        }

        protected virtual void OnBeforeSend(ref string sessionId)
        {
        }

        public void Send(Message message, TimeSpan timeout)
        {
            // apply the session from the sub class if needed
            var sessionId = string.Empty;
            OnBeforeSend(ref sessionId);
            if (message.Headers.RelatesTo != null)
            {
                // when relatesTo is not null it means this is a response message which must be send to the request channel
                // and after sent out the original request had been finished so we don't need to store the original message any more
                // hence we will remove and retrieve the original message id and append to the bus message and send out
                var replyTo = string.Empty;
                _replyTos.TryRemove(message.Headers.RelatesTo, out replyTo);
                if (!string.IsNullOrWhiteSpace(replyTo))
                {
                    var content = GetStringFromWcfMessage(message, RemoteAddress);
                    _bus.SendReply(content, sessionId, _isClient, replyTo);
                }
                else
                {
                    throw new CommunicationException(string.Format("Cannot find the ReplyTo valid for the message related to {0}.", message.Headers.RelatesTo));
                }
            }
            else
            {
                // on the server side, when performing the callback request we will firstly retrieve the original request message id, 
                // then find the related client channel id. so that we can send the callback request back to the same client channel.
                var sendTo = string.Empty;
                if (!_isClient &&
                    OperationContext.Current != null &&
                    OperationContext.Current.RequestContext != null &&
                    OperationContext.Current.RequestContext.RequestMessage != null &&
                    OperationContext.Current.RequestContext.RequestMessage.Headers.MessageId != null)
                {
                    var requestMessageId = OperationContext.Current.RequestContext.RequestMessage.Headers.MessageId;
                    _replyTos.TryGetValue(requestMessageId, out sendTo);
                }
                var content = GetStringFromWcfMessage(message, RemoteAddress);
                _bus.SendRequest(content, sessionId, _isClient, ChannelID, sendTo);
            }
        }

        public void Send(Message message)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginReceive(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginReceive(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginWaitForMessage(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public Message EndReceive(IAsyncResult result)
        {
            throw new NotImplementedException();
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

        public IAsyncResult BeginSend(Message message, TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginSend(Message message, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void EndSend(IAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }
}
