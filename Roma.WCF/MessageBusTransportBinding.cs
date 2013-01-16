using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;
using Roma.Bus;

namespace Roma.WCF
{
    public class MessageBusTransportBinding : Binding
    {
        private readonly MessageEncodingBindingElement _messageElement;
        private readonly MessageBusTransportBindingElement _transportElement;
        private readonly ReliableSessionBindingElement _sessionElement;

        public MessageBusTransportBinding(IBus bus)
            : this(bus, SessionfulMode.Distributed)
        {
        }

        public MessageBusTransportBinding(IBus bus, SessionfulMode sessionfulMode)
            : base()
        {
            _messageElement = new TextMessageEncodingBindingElement();
            _transportElement = new MessageBusTransportBindingElement(bus);
            if (sessionfulMode == SessionfulMode.Standard)
            {
                _sessionElement = new ReliableSessionBindingElement();
            }
        }

        public override BindingElementCollection CreateBindingElements()
        {
            var elements = new BindingElementCollection();
            elements.Add(_messageElement);
            if (_sessionElement != null)
            {
                elements.Add(_sessionElement);
            }
            // the transport binding element must be the last one
            elements.Add(_transportElement);
            return elements.Clone();
        }

        public override string Scheme
        {
            get
            {
                return _transportElement.Scheme;
            }
        }
    }
}
