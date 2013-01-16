using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Roma.Bus
{
    public interface IBus : IDisposable
    {
        string SendRequest(string message, string sessionId, bool fromClient, string from, string to = null);

        void SendReply(string message, string sessionId, bool fromClient, string replyTo);

        BusMessage Receive(bool fromClient, string replyTo);
    }
}
