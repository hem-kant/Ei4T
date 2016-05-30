using Ei4T.Common.Model;
using Ei4T.CoreServiceClientL.CoreServiceFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Tridion.ContentManager.CoreService.Client;
using Ei4T.CoreServiceClient.helper;

namespace Ei4T.CoreServiceClient
{
    public class Generation
    {
        public static ICoreServiceFrameworkContext coreService = null;
        public static void process(List<News> DataFromExchange)
        {

            try
            {
                coreService = CoreServiceFactory.GetCoreServiceContext(new Uri(ConfigurationSettings.AppSettings["CoreServiceURL"].ToString()), new NetworkCredential(ConfigurationSettings.AppSettings["UserName"].ToString(), ConfigurationSettings.AppSettings["Password"].ToString(), ConfigurationSettings.AppSettings["Domain"].ToString()));
                SchemaFieldsData schemaFieldData = coreService.Client.ReadSchemaFields(ConfigurationSettings.AppSettings["SchemaID"].ToString(), true, new ReadOptions());
                foreach (News p in DataFromExchange)
                {
                    string serializeXml = "";
                    bool bln = helper.helper.Serialize<News>(p, ref serializeXml);
                    string xml = serializeXml;
                    var tcmuri = TridionComponent.GenerateComponent(coreService, xml, helper.helper.SetPublication(ConfigurationSettings.AppSettings["FolderLocation"].ToString(), ConfigurationSettings.AppSettings["SchemaID"].ToString()), helper.helper.SchemaType.Component, ConfigurationSettings.AppSettings["FolderLocation"].ToString(), p.title, p.title);
                    if (p.IsPublish.ToLower()=="true")
                    {
                        TridionComponent.Publish(tcmuri, ConfigurationSettings.AppSettings["PublicationTargetIDs"].ToString(), coreService);
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
