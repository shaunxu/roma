using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace Roma.WCF
{
    public abstract class MessageBusChannelBase : ChannelBase
    {
        private const int CST_MAXBUFFERSIZE = 64 * 1024;

        private readonly Guid _id;
        private readonly BufferManager _bufferManager;
        private readonly MessageEncoder _encoder;

        protected BufferManager BufferManager
        {
            get
            {
                return _bufferManager;
            }
        }

        protected MessageEncoder Encoder
        {
            get
            {
                return _encoder;
            }
        }

        public string ChannelID
        {
            get
            {
                return _id.ToString();
            }
        }

        protected MessageBusChannelBase(BufferManager bufferManager, MessageEncoderFactory encoder, ChannelManagerBase parent)
            : base(parent)
        {
            _id = Guid.NewGuid();

            _bufferManager = bufferManager;
            _encoder = encoder.CreateSessionEncoder();
        }

        internal Message GetWcfMessageFromString(string content)
        {
            var raw = Encoding.UTF8.GetBytes(content);
            var data = _bufferManager.TakeBuffer(raw.Length);
            Buffer.BlockCopy(raw, 0, data, 0, raw.Length);
            var buffer = new ArraySegment<byte>(data, 0, raw.Length);
            var message = _encoder.ReadMessage(buffer, _bufferManager);
            return message;
        }

        internal string GetStringFromWcfMessage(Message message, EndpointAddress to)
        {
            ArraySegment<byte> buffer;
            string content;
            using (message)
            {
                to.ApplyTo(message);
                buffer = _encoder.WriteMessage(message, CST_MAXBUFFERSIZE, _bufferManager);
            }
            content = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            _bufferManager.ReturnBuffer(buffer.Array);
            return content;
        }

        protected override void OnAbort()
        {
        }

        protected override void OnClose(TimeSpan timeout)
        {
        }

        protected override void OnOpen(TimeSpan timeout)
        {
        }

        #region Not Implemented

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

        #endregion
    }
}
