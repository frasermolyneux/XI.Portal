using System.Threading.Tasks;

namespace XI.Portal.Library.Email
{
    public interface IEmailClient
    {
        Task SendEmailNotification(string email, EmailTemplate emailTemplate, params object[] replacements);
    }
}