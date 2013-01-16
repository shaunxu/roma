using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel;
using Roma.Bus;
using System.Xml;

namespace Roma.WCF
{
    public class MessageBusRequestSessionChannel : MessageBusRequestChannel, IRequestSessionChannel
    {
        private IOutputSession _session;

        public IOutputSession Session
        {
            get
            {
                return _session;
            }
        }

        public MessageBusRequestSessionChannel(
            BufferManager bufferManager, MessageEncoderFactory encoder, ChannelManagerBase parent, 
            EndpointAddress remoteAddress, Uri via,
            IBus bus)
            : base(bufferManager, encoder, parent, remoteAddress, via, bus)
        {
            _session = new MessageBusOutputSession((new UniqueId()).ToString());
        }

        protected override void OnBeforeRequest(ref string sessionId)
        {
            sessionId = _session.Id;
        }
    }
}
