using System;
using System.Threading.Tasks;
using FastSms;
using FastSmsSdk.Models.Requests;
using Microsoft.AspNet.Identity;

namespace XI.Portal.Library.Sms
{
    public class SmsService : IIdentityMessageService
    {
        private readonly SmsServiceConfig smsServiceConfig;

        public SmsService(SmsServiceConfig smsServiceConfig)
        {
            this.smsServiceConfig = smsServiceConfig ?? throw new ArgumentNullException(nameof(smsServiceConfig));
        }

        public Task SendAsync(IdentityMessage message)
        {
            var client = new FastSmsClient(smsServiceConfig.FastSmsClientSecret);

            var result = client.SendMessage(new SendMessageToUserRequest
            {
                SourceAddress = smsServiceConfig.SmsClientSourceAddress,
                DestinationAddress = "TEST",
                Body = message.Body
            });

            return Task.FromResult(0);
        }
    }
}