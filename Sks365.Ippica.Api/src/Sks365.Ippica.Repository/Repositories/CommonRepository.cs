using Dapper;
using Sks365Ippica.Domain.Model;
using Sks365Ippica.Domain.Model.Enums;
using Sks365Ippica.Repository.Abstraction;
using System;
using System.Collections.Generic;

namespace Sks365Ippica.Repository.Repositories
{
    internal class CommonRepository : ICommonRepository
    {
        private readonly IDataContext _dataContext;

        public CommonRepository(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }


        public ICollection<Translation> GetTranslationByCode(BookmakerEnum bookmakerId, LanguageEnum languageId, string translationCode)
        {
            var pars = new DynamicParameters(new
            {
                IDBookmaker = bookmakerId,
                IDLingua = languageId,
                CodiceStringa = translationCode
            });

            Func<IDictionary<string, object>, Translation> mapper = x => new Translation()
            {
                Code = x["CodiceStringa"] == null ? null : x["CodiceStringa"].ToString(),
                Text = x["Stringa"] == null ? null : x["Stringa"].ToString(),
                Language = languageId
            };

            var res = _dataContext.ExecuteReaderProcedure("Interfaccia.proc_StringheByLanguageId", parameters: pars, mapper: mapper);


            return res;
        }

        public ICollection<Translation> GetTranslationsFromPagine(BookmakerEnum bookmakerId, LanguageEnum languageId, string Term)
        {
            var pars = new DynamicParameters(new
            {
                IDBookmaker = bookmakerId,
                IDLingua = languageId,
                CodiceTipoStringa = Term,
                DebugMode = 0
            });

            Func<IDictionary<string, object>, Translation> mapper = x => new Translation()
            {
                TypeId = x["IDTipoPagina"] == null ? null : (int?)x["IDTipoPagina"],
                Language = x["IDLingua"] == null ? null : ((LanguageEnum?)(byte?)x["IDLingua"]),
                Text = x["Testo"] == null ? null : x["Testo"].ToString(),
                ModifyDate = x["DataUltimaModifica"] == null ? null : (DateTime?)x["DataUltimaModifica"]
            };

            var res = _dataContext.ExecuteReaderProcedure("Interfaccia.proc_Pagine_FindByTipoStringa", parameters: pars, mapper: mapper);

            return res;
        }
    }
}
