using Sks365.Ippica.Domain.Model;

namespace Sks365.Ippica.Application.Utility.EmailSender
{
    public interface IEmailSender
    {
        void SendEmail(Email email, EmailRepetitionEnum repetition);
    }
}
