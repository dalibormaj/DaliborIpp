using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System.Collections.Generic;

namespace Sks365.Ippica.DataAccess.Repositories.Abstraction
{
    public interface ICommonRepository : IRepository
    {
        List<Translation> GetTranslationByCode(BookmakerEnum bookmakerId, LanguageEnum languageId, string translationCode);
        List<Translation> GetTranslationsFromPagine(BookmakerEnum bookmakerId, LanguageEnum languageId, string Term);
        void SendEmail(string from, string to, string cc, string subject, string message);
    }
}
