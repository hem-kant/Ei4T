using System;
using Microsoft.Exchange.WebServices.Data;
using System.Configuration;

namespace Ei4T.ExchangeAuthencation
{
    public interface IUserData
    {
        ExchangeVersion Version { get; }
        string EmailAddress { get; }
        string Password { get; }
        Uri AutodiscoverUrl { get; set; }
    }

    public class UserDataFromConsole : IUserData
    {
        public static UserDataFromConsole UserData;

        public static IUserData GetUserData()
        {
            if (UserData == null)
            {
                GetUserDataFromConsole();
            }

            return UserData;
        }

        private static void GetUserDataFromConsole()
        {
            UserData = new UserDataFromConsole();

            UserData.EmailAddress = ConfigurationSettings.AppSettings["UserEmailId"].ToString();
            UserData.Password = ConfigurationSettings.AppSettings["Password"].ToString();


        }

        public ExchangeVersion Version { get { return ExchangeVersion.Exchange2013; } }

        public string EmailAddress
        {
            get;
            private set;
        }

        public string Password
        {
            get;
            private set;
        }

        public Uri AutodiscoverUrl
        {
            get;
            set;
        }
    }
}
