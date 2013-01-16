using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roma.Bus.InProcBus
{
    public class InProcMessageEntity
    {
        public Guid ID { get; set; }
        public string SessionID { get; set; }
        public string Content { get; set; }
        public bool FromClient { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public InProcMessageEntity()
            : this(string.Empty, string.Empty, false, string.Empty, string.Empty)
        {
        }

        public InProcMessageEntity(string content, string sessionId, bool fromClient, string from, string to)
        {
            ID = Guid.NewGuid();
            SessionID = sessionId;
            Content = content;
            FromClient = fromClient;
            From = from;
            To = to;
        }
    }
}
