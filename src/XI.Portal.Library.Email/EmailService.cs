using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace XI.Portal.Library.Email
{
    public class EmailService : IIdentityMessageService
    {
        private readonly IEmailClient emailClient;

        public EmailService(IEmailClient emailClient)
        {
            this.emailClient = emailClient ?? throw new ArgumentNullException(nameof(emailClient));
        }

        public Task SendAsync(IdentityMessage message)
        {
            return emailClient.SendEmailNotification(message.Destination, EmailTemplate.Standard,
                message.Subject, message.Body);
        }
    }
}