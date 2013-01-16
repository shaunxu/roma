using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Channels;

namespace Roma.WCF
{
    internal class MessageBusOutputSession : IOutputSession
    {
        private string _id;

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public MessageBusOutputSession(string id)
        {
            _id = id;
        }
    }

    internal class MessageBusInputSession : IInputSession
    {
        private string _id;

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public MessageBusInputSession(string id)
        {
            _id = id;
        }
    }

    internal class MessageBusDuplexSession : IDuplexSession
    {
        private string _id;

        public MessageBusDuplexSession(string id)
        {
            _id = id;
        }

        public IAsyncResult BeginCloseOutputSession(TimeSpan timeout, AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginCloseOutputSession(AsyncCallback callback, object state)
        {
            throw new NotImplementedException();
        }

        public void CloseOutputSession(TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public void CloseOutputSession()
        {
            throw new NotImplementedException();
        }

        public void EndCloseOutputSession(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }
    }
}
