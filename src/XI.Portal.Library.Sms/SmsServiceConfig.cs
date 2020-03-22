using System.Configuration;

namespace XI.Portal.Library.Sms
{
    public class SmsServiceConfig
    {
        public string FastSmsClientSecret => ConfigurationManager.AppSettings["FastSmsClientSecret"];
        public string SmsClientSourceAddress => ConfigurationManager.AppSettings["SmsClientSourceAddress"];
    }
}