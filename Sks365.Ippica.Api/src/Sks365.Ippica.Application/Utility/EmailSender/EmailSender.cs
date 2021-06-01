using Microsoft.AspNetCore.Http;
using Sks365.Ippica.DataAccess;
using Sks365.Ippica.Domain.Model;
using System;

namespace Sks365.Ippica.Application.Utility.EmailSender
{
    public class EmailSender : IEmailSender
    {
        private readonly IServiceProvider _serviceProvider;

        public EmailSender(IHttpContextAccessor context)
        {
            _serviceProvider = context.HttpContext.RequestServices;
        }

        public void SendEmail(Email email, EmailRepetitionEnum repetition = EmailRepetitionEnum.OnePerDay)
        {
            var _isbetsUnitOfWork = _serviceProvider.GetService(typeof(IIsbetsUnitOfWork)) as IIsbetsUnitOfWork;
            var _mstUnitOfWork = _serviceProvider.GetService(typeof(IMstUnitOfWork)) as IMstUnitOfWork;

            var canBeSent = !string.IsNullOrWhiteSpace(email.From) && !string.IsNullOrWhiteSpace(email.To);
            if (repetition == EmailRepetitionEnum.OnePerDay)
            {
                canBeSent = !_mstUnitOfWork.BetRepository.IsEmailSentToday(email);
            }

            if (canBeSent)
            {
                _isbetsUnitOfWork.CommonRepository.SendEmail(email.From, email.To, email.Cc, email.Subject, email.Message);
                _mstUnitOfWork.BetRepository.InsertLogEmail(email);
            }
        }
    }
}
