using Ei4T.CoreServiceClientL.CoreServiceFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Tridion.ContentManager.CoreService.Client;

namespace Ei4T.CoreServiceClient.helper
{
    public class helper
    {
        public enum SchemaType { Component, Metadata, Multimedia, None };
        #region Deserialize Method
        public static T Deserialize<T>(XmlDocument xmlDocument)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));

            StringReader reader = new StringReader(xmlDocument.InnerXml);
            XmlReader xmlReader = new XmlTextReader(reader);
            //Deserialize the object.
            return (T)ser.Deserialize(xmlReader);
        }
        public static T GetObject<T>(Dictionary<string, string> dict)
        {
            Type type = typeof(T);
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                type.GetProperty(kv.Key).SetValue(obj, kv.Value);
            }
            return (T)obj;
        }

        public static bool Serialize<T>(T value, ref string serializeXml)
        {
            try
            {
                XmlSerializer xmlserializer = new XmlSerializer(typeof(T));
                StringWriter stringWriter = new StringWriter();
                XmlWriter writer = XmlWriter.Create(stringWriter);

                xmlserializer.Serialize(writer, value);

                serializeXml = stringWriter.ToString();

                writer.Close();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        #endregion
        #region Remove All Namespaces
        //Implemented based on interface, not part of algorithm
        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));
            return xmlDocumentWithoutNs.ToString();
        }


        //Core recursion function
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }
        #endregion

        #region Get Tridion Object
        public static TridionObjectInfo GetTridionObject(ICoreServiceFrameworkContext coreService, ItemType itemtype, string parentTcmUri, string searchtext)
        {
            TridionObjectInfo tridionObjectInfo = new TridionObjectInfo();
            try
            {
                IdentifiableObjectData tridionObject = null;
                var filter = new OrganizationalItemItemsFilterData
                {
                    Recursive = true,
                    ItemTypes = new ItemType[] { itemtype }
                };
                var pageList = coreService.Client.GetListXml(parentTcmUri, filter);

                int objectCount = (from e in pageList.Elements()
                                   where e.Attribute("Title").Value.ToLower().Equals(searchtext.ToLower()) || e.Attribute("ID").Value.ToLower() == searchtext.ToLower()
                                   select e.Attribute("ID").Value).Count();

                tridionObjectInfo.ObjectCount = objectCount;
                if (objectCount > 0)
                {
                    var objectUri = (from e in pageList.Elements()
                                     where e.Attribute("Title").Value.ToLower().Equals(searchtext.ToLower()) || e.Attribute("ID").Value.ToLower() == searchtext.ToLower()
                                     select e.Attribute("ID").Value).First();

                    tridionObject = coreService.Client.Read(objectUri, new ReadOptions
                    {
                        LoadFlags = LoadFlags.None
                    });
                    tridionObjectInfo.TridionObject = tridionObject;
                    tridionObjectInfo.TcmUri = objectUri;
                }
            }
            catch (Exception ex)
            {

            }
            return tridionObjectInfo;
        }
        #endregion
        #region Set Publication
        public static string SetPublication(string pTcmUri, string cTcmUri)
        {
            string folderpublication = pTcmUri.Substring(pTcmUri.IndexOf("tcm:"), pTcmUri.IndexOf("-"));
            string schemapublication = cTcmUri.Substring(cTcmUri.IndexOf("tcm:"), cTcmUri.IndexOf("-"));
            return cTcmUri.Replace(schemapublication, folderpublication);
        }
        #endregion
    }
}
