using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Tridion.ContentManager.CoreService.Client;

namespace Ei4T.CoreServiceClientL.CoreServiceFramework
{
    public abstract class CoreServiceFrameworkContext : ICoreServiceFrameworkContext
    {
        protected Uri _endpointUri = null;
        public Uri EndpointUri
        {
            get
            {
                return _endpointUri;
            }
        }

        protected SessionAwareCoreServiceClient _client = null;
        public ISessionAwareCoreService Client
        {
            get
            {
                return _client;
            }
        }

        /// <summary>
        /// Initialize the CoreService Context
        /// </summary>
        /// <param name="endpointUri"></param>
        /// <param name="windowsNetworkCredentials"></param>
        public CoreServiceFrameworkContext(Uri endpointUri, NetworkCredential windowsNetworkCredentials)
        {
            _endpointUri = endpointUri;

            //Dynamically instantiate the correct binding from the Abstract method that each implementing class will provide.
            var endpointBinding = this.GetBinding();

            //Create the Endpoint Address for WCF
            var endpointAddress = new EndpointAddress(endpointUri);

            //Initialize the Client Proxy and the NetworkCredentials for Connection, if NetworkCreditials are null then skip
            //and allow .Net to process with defaults.
            _client = new SessionAwareCoreServiceClient(endpointBinding, endpointAddress);

            //Implement Optimizations for the Service connection shared by all Implementations
            //NOTE:  If specified then implement NetworkSecurity Credentials
            if (windowsNetworkCredentials != null)
            {
                _client.ChannelFactory.Credentials.Windows.ClientCredential = windowsNetworkCredentials;
            }

            //NOTE:  Implement optimizations for WCF Client Connection
            //       See this Blog for more info. on WCF Optimization: http://blog.shutupandcode.net/?p=1085
            if (_client.State == CommunicationState.Created)
            {
                _client.Open();
            }

        }

        /// <summary>
        /// Abstract method that must be implemented by classes that implement this CoreServiceFrameworkContext base class.
        /// More Info: http://code.google.com/p/tridion-practice/wiki/GetCoreServiceClientWithoutConfigFile
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public abstract Binding GetBinding();

        /// <summary>
        /// This allows efficient use of the CoreServices and automatic disposal and cleanup when 
        /// properly used within using {} statements.
        /// </summary>
        public void Dispose()
        {
            if (_client != null)
            {
                switch (_client.State)
                {
                    case CommunicationState.Faulted:
                        _client.Abort();
                        break;
                    case CommunicationState.Opened:
                        _client.Close();
                        break;
                }

                //Since clientSession is by default an Interface we must polymorphically re-cast back
                //to IDisposable and then ensure it is still valid to safely dispose of the item.
                var disposable = _client as IDisposable;
                if (disposable != null) disposable.Dispose();
            }

        }

    }
}
