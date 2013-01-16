using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roma.Bus
{
    public class BusMessage
    {
        public string MessageID { get; private set; }
        public string SessionID { get; private set; }
        public string From { get; private set; }
        public string ReplyTo { get; private set; }
        public string Content { get; private set; }

        public BusMessage(string messageId, string sessionId, string fromChannelId, string replyToChannelId, string content)
        {
            MessageID = messageId;
            SessionID = sessionId;
            From = fromChannelId;
            ReplyTo = replyToChannelId;
            Content = content;
        }
    }
}
