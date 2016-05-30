using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Ei4T.CoreServiceClientL.CoreServiceFramework
{
    public class MaxxedWsHttpBinding : WSHttpBinding
    {
        public MaxxedWsHttpBinding(SecurityMode securityMode)
            : base(securityMode)
        {
            initializeMaxSettings();
        }

        public MaxxedWsHttpBinding()
            : base()
        {
            initializeMaxSettings();
        }

        private void initializeMaxSettings()
        {
            //this.MaxBufferSize = Int32.MaxValue;
            this.MaxBufferPoolSize = Int32.MaxValue;
            this.MaxReceivedMessageSize = Int32.MaxValue;
            this.ReaderQuotas = new XmlDictionaryReaderQuotas
            {
                MaxDepth = Int32.MaxValue,
                MaxStringContentLength = Int32.MaxValue,
                MaxArrayLength = Int32.MaxValue
            };
        }

    }
}
