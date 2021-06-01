using AutoMapper;
using Dapper;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.DataAccess.Repositories.Abstraction;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System.Collections.Generic;

namespace Sks365.Ippica.DataAccess.Repositories
{
    internal class CommonRepository : ICommonRepository
    {
        private readonly IDataContext _dataContext;

        public CommonRepository(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public List<Translation> GetTranslationByCode(BookmakerEnum bookmakerId, LanguageEnum languageId, string translationCode)
        {
            var pars = new DynamicParameters(new
            {
                IDBookmaker = bookmakerId,
                IDLingua = languageId,
                CodiceStringa = translationCode
            });

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, Translation>()
                   .ForMember(d => d.Code, opt => opt.MapFrom(src => src["CodiceStringa"]))
                   .ForMember(d => d.Text, opt => opt.MapFrom(src => src["Stringa"]))
                   .ForMember(d => d.LanguageId, opt => opt.MapFrom(src => languageId));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<Translation>("Interfaccia.proc_StringheByLanguageId", mapper, pars);

            return res;
        }

        public List<Translation> GetTranslationsFromPagine(BookmakerEnum bookmakerId, LanguageEnum languageId, string Term)
        {
            var pars = new DynamicParameters(new
            {
                IDBookmaker = bookmakerId,
                IDLingua = languageId,
                CodiceTipoStringa = Term,
                DebugMode = 0
            });

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, Translation>()
                   .ForMember(d => d.TypeId, opt => opt.MapFrom(src => src["IDTipoPagina"]))
                   .ForMember(d => d.LanguageId, opt => opt.MapFrom(src => src["IDLingua"]))
                   .ForMember(d => d.Text, opt => opt.MapFrom(src => src["Testo"]))
                   .ForMember(d => d.ModifyDate, opt => opt.MapFrom(src => src["DataUltimaModifica"]));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<Translation>("Interfaccia.proc_Pagine_FindByTipoStringa", mapper, pars);

            return res;
        }

        public void SendEmail(string from, string to, string cc, string subject, string message)
        {
            from = from ?? string.Empty;
            to = to?.Replace(";", ",") ?? string.Empty;
            cc = cc?.Replace(";", ",") ?? string.Empty;
            subject = subject ?? string.Empty;
            message = message ?? string.Empty;

            var pars = new DynamicParameters(new
            {
                @IndirizzoMittente = from,
                @NomeMittente = from,
                @DestinatariTo = to,
                @DestinatariCC = cc,
                @DestinatariBCC = string.Empty,
                @Titolo = subject,
                @Messaggio = message,
                @IDTipoEmail = 2
            });

            var res = _dataContext.ExecuteReaderProcedure("[Messaggi].[proc_Email_InviaConServerPredefinito]", parameters: pars);
        }
    }
}
