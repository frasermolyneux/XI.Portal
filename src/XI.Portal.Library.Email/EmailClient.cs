using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace XI.Portal.Library.Email
{
    public class EmailClient : IEmailClient
    {
        private readonly EmailClientConfig emailClientConfig;

        public EmailClient(EmailClientConfig emailClientConfig)
        {
            this.emailClientConfig = emailClientConfig ?? throw new ArgumentNullException(nameof(emailClientConfig));
        }

        private string StandardNotificationBody =>
            "Hi,<br>" +
            "<br>" +
            "{1}<br>" +
            "<br>" +
            "- " + emailClientConfig.SmtpWebsiteName + " Notifications";

        private string ChangePasswordNotificationBody =>
            "Hi {0},<br>" +
            "<br>" +
            "Your password has been changed on the " + emailClientConfig.SmtpWebsiteName + " - contact the admin team for support if this was not you. <br>" +
            "<br>" +
            "- " + emailClientConfig.SmtpWebsiteName + " Notifications";

        private string TemporaryPasswordNotificationBody =>
            "Hi {0},<br>" +
            "<br>" +
            "Your temporary password has been set on the " + emailClientConfig.SmtpWebsiteName + ", please login and change your password to a memorable one. <br>" +
            "<br>" +
            "Your username is: {0}<br>" +
            "Your password is: {1}<br>" +
            "<br>" +
            "- " + emailClientConfig.SmtpWebsiteName + " Notifications";

        private string NewNotificationBody =>
            "Hi {0},<br>" +
            "<br>" +
            "You have a new notification in the " + emailClientConfig.SmtpWebsiteName + ": <br>" +
            "<br>" +
            "{1}<br>" +
            "<br>" +
            "- " + emailClientConfig.SmtpWebsiteName + " Notifications";

        public Task SendEmailNotification(string email, EmailTemplate emailTemplate, params object[] replacements)
        {
            switch (emailTemplate)
            {
                case EmailTemplate.Standard:
                    return SendEmail(email, string.Format("{0}", replacements),
                        string.Format(StandardNotificationBody, replacements));
                case EmailTemplate.ChangedPasswordNotification:
                    return SendEmail(email, $"{emailClientConfig.SmtpWebsiteName} Password Change",
                        string.Format(ChangePasswordNotificationBody, replacements));
                case EmailTemplate.TemporaryAccountPasswordNotification:
                    return SendEmail(email, $"{emailClientConfig.SmtpWebsiteName} Temporary Password",
                        string.Format(TemporaryPasswordNotificationBody, replacements));
                case EmailTemplate.NewNotification:
                    return SendEmail(email, $"{emailClientConfig.SmtpWebsiteName} Notification",
                        string.Format(NewNotificationBody, replacements));
                default:
                    throw new ArgumentOutOfRangeException(nameof(emailTemplate), emailTemplate, null);
            }
        }

        public Task SendEmail(string destination, string subject, string body)
        {
            var client = new SmtpClient(emailClientConfig.SmtpServer, emailClientConfig.SmtpPort);
            var clientCredentials =
                new NetworkCredential(emailClientConfig.SmtpUsername, emailClientConfig.SmtpPassword);

            client.UseDefaultCredentials = false;
            client.Credentials = clientCredentials;

            var from = new MailAddress(emailClientConfig.SmtpFrom, emailClientConfig.SmtpFromName, Encoding.UTF8);
            var to = new MailAddress(destination);

            var message = new MailMessage(from, to)
            {
                Body = body,
                Subject = subject,
                SubjectEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };

            client.Send(message);

            return Task.FromResult(0);
        }
    }
}