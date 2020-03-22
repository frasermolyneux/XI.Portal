using System;
using System.Configuration;

namespace XI.Portal.Library.Email
{
    public class EmailClientConfig
    {
        public string SmtpServer => ConfigurationManager.AppSettings["SmtpServer"];
        public int SmtpPort => Convert.ToInt32(ConfigurationManager.AppSettings["SmtpPort"]);
        public string SmtpUsername => ConfigurationManager.AppSettings["SmtpUsername"];
        public string SmtpPassword => ConfigurationManager.AppSettings["SmtpPassword"];
        public string SmtpFrom => ConfigurationManager.AppSettings["SmtpFrom"];
        public string SmtpFromName => ConfigurationManager.AppSettings["SmtpFromName"];
        public string SmtpWebsiteName => ConfigurationManager.AppSettings["SmtpWebsiteName"];
    }
}