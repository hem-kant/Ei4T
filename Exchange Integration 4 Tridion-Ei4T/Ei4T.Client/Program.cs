using Ei4T.Common.Model;
using Ei4T.CoreServiceClient;
using Ei4T.CoreServiceClient.helper;
using Ei4T.ExchangeAuthencation;
using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Ei4T.Client
{
    class Program
    {
        static void Main(string[] args)
        {
 
            // Create an instance of the custom URL redirection validator.
            UrlValidator validator = new UrlValidator();

            // Get the user's email address and password from the console.
            IUserData userData = UserDataFromConsole.GetUserData();

            // Create an ExchangeService object with the user's credentials.
            ExchangeService myService = new ExchangeService();
            myService.Credentials = new NetworkCredential(userData.EmailAddress, userData.Password);


            Console.WriteLine("Getting EWS URL using custom validator...");

            // Call the Autodisocer service with the custom URL validator.
            myService.AutodiscoverUrl(userData.EmailAddress, validator.ValidateUrl);
            TimeSpan ts = new TimeSpan(0, -1, 0, 0);
            DateTime date = DateTime.Now.Add(ts);
            PropertySet itempropertyset = new PropertySet(BasePropertySet.FirstClassProperties);
            itempropertyset.RequestedBodyType = BodyType.Text;
            SearchFilter.IsGreaterThanOrEqualTo filter = new SearchFilter.IsGreaterThanOrEqualTo(ItemSchema.DateTimeReceived, date);

            if (myService != null)
            {
                ItemView itemview = new ItemView(1000);
                itemview.PropertySet = itempropertyset;

                FindItemsResults<Item> findResults = myService.FindItems(WellKnownFolderName.Inbox, filter, itemview);
                List<News> newsList = new List<News>();
                if (findResults.Items.Count > 0)
                {
                    foreach (Item item in findResults)
                    {
                        item.Load(itempropertyset);
                        string Body = item.Body.Text.ToString().Replace("\r\n", "");
                        string[] emailBody = Body.Split('|');
                        Dictionary<string, string> dictionary = new Dictionary<string, string>();
                        foreach (var item11 in emailBody)
                        {
                            dictionary.Add(item11.Substring(0, item11.LastIndexOf(':')).Trim(), item11.Substring(item11.LastIndexOf(':') + 1).Trim());
                        }
                        dictionary.Add("IsPublish", item.Importance.ToString() == "High" ? "True" : "False");
                        var test = helper.GetObject<News>(dictionary);
                        newsList.Add(test);

                    }
                    Generation.process(newsList);
                }

            }
        }
    }
}
