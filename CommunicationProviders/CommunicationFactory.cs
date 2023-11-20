using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmYdm.Providers.CommunicationProviders
{
    public sealed class CommunicationFactory
    {
        public const string OPTIONS_TEST = "test";
        public const string OPTIONS_LOCAL = "local";
        public const string OPTIONS_MICROPAY = "micropay";
        public const string OPTIONS_GMAIL = "gmail";


        public static string Domain = "";

        private static string DEFULT_CONFIG = "";

        public static GmailConfig GmailConfig = null;
        public static MicropayConfig MicropayConfig = null;

        public static IMailSender GetMailInstance(string type = "")
        {
            if (string.IsNullOrEmpty(type))
                type = DEFULT_CONFIG;

            switch (type.ToLower())
            {
                case OPTIONS_LOCAL:
                    return new MailLocalhost();
                case OPTIONS_TEST:
                    return new TestEnvironmentMailAndSMSProvider();
                default:
                    return new MailGmailProvider();
            }
        }

        public static ISmsSender GetSmsInstance(string type = "")
        {
            if (string.IsNullOrEmpty(type))
                type = DEFULT_CONFIG;

            switch (type.ToLower())
            {
                case OPTIONS_TEST:
                    return new TestEnvironmentMailAndSMSProvider();
                default:
                    return new SmsMicropayProvider(MicropayConfig.Number, MicropayConfig.UserID, MicropayConfig.UserName);
            }
        }

        public static void SetDefultConfig(string defultOpt)
        {
            DEFULT_CONFIG = defultOpt;
        }

        public static void SetDomainContext(string domain)
        {
            Domain = domain;
        }
    }

    public class GmailConfig
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class MicropayConfig
    {
        public string UserName { get; set; }
        public string UserID { get; set; }
        public string Number { get; set; }
    }
}
