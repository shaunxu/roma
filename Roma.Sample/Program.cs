using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Roma.WCF;
using Roma.Bus.InProcBus;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Threading;
using Roma.Bus;
using System.Collections.Concurrent;

namespace Roma.Sample
{
    public class InProcSessionStore
    {
        #region Singleton

        private static InProcSessionStore _instance;

        public static InProcSessionStore Current
        {
            get
            {
                return _instance;
            }
        }

        static InProcSessionStore()
        {
            _instance = new InProcSessionStore();
        }

        private InProcSessionStore()
        {
            _dic = new ConcurrentDictionary<string, object>();
        }

        #endregion

        private ConcurrentDictionary<string, object> _dic;

        public void Set(string key, object value)
        {
            _dic.AddOrUpdate(key, value, (k, v) => value);
        }

        public T Get<T>(string key)
        {
            var value = default(T);
            object result;
            if (_dic.TryGetValue(key, out result) && result != null && result.GetType() == typeof(T))
            {
                value = (T)result;
            }
            return value;
        }
    }

    [ServiceContract(Namespace = "http://wcf.shaunxu.me/", SessionMode= SessionMode.Required)]
    public interface ISampleService
    {
        [OperationContract(IsOneWay = true)]
        void Add(int value);

        [OperationContract(IsOneWay = true)]
        void GetResult(string id);
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class SampleService : ISampleService
    {
        public void Add(int value)
        {
            //_current += value;
            var current = InProcSessionStore.Current.Get<int>(OperationContext.Current.SessionId);
            current += value;
            InProcSessionStore.Current.Set(OperationContext.Current.SessionId, current);
            Console.WriteLine("[{0}] SampleService.Add({1}), Current = {2}, SessionID = {3}", this.GetHashCode(), value, current, OperationContext.Current.SessionId);
        }

        public void GetResult(string id)
        {
            var current = InProcSessionStore.Current.Get<int>(OperationContext.Current.SessionId);
            Console.WriteLine("[{0}] SampleService.GetResult(), SessionID = {1}", this.GetHashCode(), OperationContext.Current.SessionId);
            InProcSessionStore.Current.Set(id, current);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var bus = new InProcMessageBus();
            var address = "net.bus://localhost/sample";

            // establish the services
            var host1 = EstablishServiceHost<ISampleService, SampleService>(bus, address, SessionfulMode.Distributed);
            var host2 = EstablishServiceHost<ISampleService, SampleService>(bus, address, SessionfulMode.Distributed);

            // establish the client
            var client1 = EstablishClientProxy<ISampleService>(bus, address, SessionfulMode.Distributed);
            var client2 = EstablishClientProxy<ISampleService>(bus, address, SessionfulMode.Distributed);
            using (client1 as IDisposable)
            using (client2 as IDisposable)
            {
                client1.Add(1);
                client2.Add(4);
                
                client1.Add(3);
                client2.Add(5);
                
                client1.Add(2);
                client2.Add(6);

                client1.GetResult("1");
                client2.GetResult("2");
                Console.WriteLine("Client 1 Result: {0}", InProcSessionStore.Current.Get<int>("1"));
                Console.WriteLine("Client 2 Result: {0}", InProcSessionStore.Current.Get<int>("2"));
            }

            // close the service
            host1.Close();
            host2.Close();

            Console.ReadKey();
        }

        static TChannel EstablishDuplexClientProxy<TChannel, TCallback>(IBus bus, string address) where TCallback : new()
        {
            var binding = new MessageBusTransportBinding(bus);
            var callbackInstance = new InstanceContext(new TCallback());
            var factory = new DuplexChannelFactory<TChannel>(callbackInstance, binding, address);
            factory.Opened += (sender, e) =>
                {
                    Console.WriteLine("Client connected to {0}", factory.Endpoint.ListenUri);
                };
            var proxy = factory.CreateChannel();
            return proxy;
        }

        static TChannel EstablishClientProxy<TChannel>(IBus bus, string address, SessionfulMode sessionfulMode)
        {
            var binding = new MessageBusTransportBinding(bus, sessionfulMode);
            var factory = new ChannelFactory<TChannel>(binding, address);
            factory.Opened += (sender, e) =>
            {
                Console.WriteLine("Client connected to {0}", factory.Endpoint.ListenUri);
            };
            var proxy = factory.CreateChannel();
            return proxy;
        }

        static ServiceHost EstablishServiceHost<TChannel, TService>(IBus bus, string address, SessionfulMode sessionfulMode)
        {
            var host = new ServiceHost(typeof(TService));
            var binding = new MessageBusTransportBinding(bus, sessionfulMode);
            host.AddServiceEndpoint(typeof(TChannel), binding, address);
            host.Opened += (sender, e) =>
            {
                Console.WriteLine("Service ({0}) opened at {1}", host.GetHashCode(), host.Description.Endpoints[0].ListenUri);
            };
            host.Open();
            return host;
        }
    }
}
