using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Ei4T.Common.Model
{
    [DataContract]
    [Serializable]
    [XmlRoot("ExchangeNews")]
    public class News
    {
        [XmlElement("title")]
        [DataMember]
        public string title { get; set; }

        [XmlElement("description")]
        [DataMember]
        public string description { get; set; }

        [XmlElement("IsPublish")]
        [DataMember]
        public string IsPublish { get; set; }

    }
}
