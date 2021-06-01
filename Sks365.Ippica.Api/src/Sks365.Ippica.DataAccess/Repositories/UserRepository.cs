using AutoMapper;
using Dapper;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.DataAccess.Repositories.Abstraction;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sks365.Ippica.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDataContext _dataContext;

        public UserRepository(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public List<SelfLimitation> CheckSelfLimitations(int userId, decimal amount, LanguageEnum languageId)
        {
            var pars = new DynamicParameters(new
            {
                IDUtente = userId,
                Importo = amount,
                IDLingua = languageId
            });


            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, SelfLimitation>()
                   .ForMember(d => d.SelflimitationTypeId, opt => opt.MapFrom(src => src["IDTipoAutolimitazione"]))
                   .ForMember(d => d.LimitAmount, opt => opt.MapFrom(src => src["LimitAmount"]))
                   .ForMember(d => d.LimitDays, opt => opt.MapFrom(src => src["LimitDays"]))
                   .ForMember(d => d.StartDate, opt => opt.MapFrom(src => src["StartDate"]))
                   .ForMember(d => d.RemainingAmount, opt => opt.MapFrom(src => src["RemainingAmount"]))
                   .ForMember(d => d.ErrorMessage, opt => opt.MapFrom(src => src["ErrorMessage"]))
                   .ForMember(d => d.Selflimited, opt => opt.MapFrom(src => src["AutoLimited"]));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<SelfLimitation>("Utenti.proc_Autolimitazioni_Check", mapper, parameters: pars);

            return res;
        }

        public SelfLimitation CheckSelfLimitation(int userId, decimal amount, LanguageEnum languageId, SelflimitationTypeEnum selflimitationType)
        {
            var selfLimitations = CheckSelfLimitations(userId, amount, languageId);
            if (selfLimitations != null)
            {
                var limit = (from item in selfLimitations
                             where item.SelflimitationTypeId == selflimitationType
                             select item).FirstOrDefault();

                return limit;
            }

            return null;
        }

        public List<User> GetUsers(int? userId = null, string userName = "", BookmakerEnum? bookmakerId = null)
        {
            if (userId == null && string.IsNullOrEmpty(userName) && bookmakerId == null)
            {
                throw new ArgumentNullException("Parameter data missing. Please pass one of the following values: userId, userName or bookmakerId!");
            }

            var pars = new DynamicParameters();

            pars.Add(name: "@switch", dbType: System.Data.DbType.Byte, value: 0);

            if (userId != null)
                pars.Add(name: "@IDUtente", dbType: System.Data.DbType.Int32, value: userId);
            if (!string.IsNullOrEmpty(userName))
                pars.Add(name: "@Uid", dbType: System.Data.DbType.String, value: userName);
            if (bookmakerId != null)
                pars.Add(name: "@IDBookmaker", dbType: System.Data.DbType.Int16, value: bookmakerId);

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, User>()
                   .ForMember(d => d.UserId, opt => opt.MapFrom(src => src["IDUtente"]))
                   .ForMember(d => d.BookmakerId, opt => opt.MapFrom(src => src["IDBookmaker"]))
                   .ForMember(d => d.ParentId, opt => opt.MapFrom(src => src["IDPadre"]))
                   .ForMember(d => d.Username, opt => opt.MapFrom(src => src["Uid"]))
                   .ForMember(d => d.Password, opt => opt.MapFrom(src => src["Pwd"]))
                   .ForMember(d => d.Name, opt => opt.MapFrom(src => src["Nome"]))
                   .ForMember(d => d.Surname, opt => opt.MapFrom(src => src["Cognome"]))
                   .ForMember(d => d.Status, opt => opt.MapFrom(src => src["Stato"]))
                   .ForMember(d => d.IsTestUser, opt => opt.MapFrom(src => src["TestUser"]))
                   .ForMember(d => d.UserTypeId, opt => opt.MapFrom(src => src["TipoUtente"]))
                   .ForMember(d => d.Description, opt => opt.MapFrom(src => src["Descrizione"]))
                   .ForMember(d => d.UserTypeDescription, opt => opt.MapFrom(src => src["TipoUtenteDesc"]))
                   .ForMember(d => d.AccountCurrencyId, opt => opt.MapFrom(src => src["IDValutaConto"]));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<User>("dbo.proc_Utenti", mapper, parameters: pars);

            return res;
        }

        public User GetUser(int userId)
        {
            var user = GetUsers(userId, string.Empty).FirstOrDefault();
            return user;
        }

        public User GetUser(string userName, BookmakerEnum bookmakerId)
        {
            var user = GetUsers(userName: userName, bookmakerId: bookmakerId)?.FirstOrDefault();
            return user;
        }

        public UserDetail GetUserDetails(int userId)
        {
            var pars = new DynamicParameters(new
            {
                IDUtente = userId
            });

            pars.Add(name: "@switch", dbType: System.Data.DbType.Byte, value: 0);

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, UserDetail>()
                   .ForMember(d => d.UserId, opt => opt.MapFrom(src => src["IDUtente"]))
                   .ForMember(d => d.Note, opt => opt.MapFrom(src => src["Note"]))
                   .ForMember(d => d.ReservedNote, opt => opt.MapFrom(src => src["NoteRiserva"]))
                   .ForMember(d => d.Color, opt => opt.MapFrom(src => src["Colore"]))
                   .ForMember(d => d.LastLogin, opt => opt.MapFrom(src => src["UltimoLogIn"]))
                   .ForMember(d => d.CountryId, opt => opt.MapFrom(src => src["IDPaese"]))
                   .ForMember(d => d.Address, opt => opt.MapFrom(src => src["Indirizzo"]))
                   .ForMember(d => d.City, opt => opt.MapFrom(src => src["Citta"]))
                   .ForMember(d => d.PostalCode, opt => opt.MapFrom(src => src["Cap"]))
                   .ForMember(d => d.Province, opt => opt.MapFrom(src => src["Provincia"]))
                   .ForMember(d => d.Region, opt => opt.MapFrom(src => src["Regione"]))
                   .ForMember(d => d.Telephone, opt => opt.MapFrom(src => src["Telefono"]))
                   .ForMember(d => d.MobilePhone, opt => opt.MapFrom(src => src["Cellulare"]))
                   .ForMember(d => d.BirthDate, opt => opt.MapFrom(src => src["DataNascita"]))
                   .ForMember(d => d.Email, opt => opt.MapFrom(src => src["Email"]))
                   .ForMember(d => d.CreationDate, opt => opt.MapFrom(src => src["DataCreazione"]))
                   .ForMember(d => d.SecurityQuestionId, opt => opt.MapFrom(src => src["IDDomandaSicurezza"]))
                   .ForMember(d => d.SecurityAnswer, opt => opt.MapFrom(src => src["RipostaSicurezza"]))
                   .ForMember(d => d.IDGMT, opt => opt.MapFrom(src => src["IDGMT"]))
                   .ForMember(d => d.InternationalCountryCodeId, opt => opt.MapFrom(src => src["IDPaeseInternazionaleCodice"]))
                   .ForMember(d => d.BetsReserveUserTypeId, opt => opt.MapFrom(src => src["IDTipoUtentiScommesseRiserva"]));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<UserDetail>("dbo.proc_UtentiDettaglio", mapper, parameters: pars).FirstOrDefault();

            return res;
        }

        public List<UserAdditionalData> GetUserAdditionalData(int userId)
        {
            var pars = new DynamicParameters(new
            {
                IDUtente = userId
            });

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, UserAdditionalData>()
                   .ForMember(d => d.UserDataTypeId, opt => opt.MapFrom(src => src["IDTipoDatoUtente"]))
                   .ForMember(d => d.Value, opt => opt.MapFrom(src => src["Valore"]));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<UserAdditionalData>("dbo.proc_UtentiDatiAggiuntivi_ListByIDUtente", parameters: pars, mapper: mapper);

            return res;
        }

        public UserAdditionalData GetUserAdditionalDataValue(int userId, UserDataTypeEnum userDataType)
        {
            var pars = new DynamicParameters(new
            {
                IDUtente = userId,
                IDTipoDatoUtente = userDataType
            });

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, UserAdditionalData>()
                   .ForMember(d => d.UserDataTypeId, opt => opt.MapFrom(src => userDataType))
                   .ForMember(d => d.Value, opt => opt.MapFrom(src => src["Valore"]));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<UserAdditionalData>("api.proc_Utenti_GetDatoAggiuntivo", parameters: pars, mapper: mapper);

            return res?.FirstOrDefault();
        }

        public string GetUserParameterValue(int userId, UserParameterTypeEnum? userParameterId)
        {
            var pars = new DynamicParameters(new
            {
                IDUtente = userId,
                IDTipoParametroUtente = userParameterId
            });

            var res = _dataContext.ExecuteReaderProcedure("Configurazione.proc_ParametriUtente_Find", parameters: pars).FirstOrDefault();
            var valore = res["Valore"]?.ToString() ?? null;

            return valore;
        }

        public bool GetUserAvailibility(int userId, BookmakerEnum bookmakerId, ProviderEnum providerId, SectionTypeEnum sectionType)
        {
            bool retVal = false;

            var pars = new DynamicParameters(new
            {
                IDBookmaker = bookmakerId,
                IDProvider = providerId,
                IDTipoSezione = sectionType,
                IDUtente = userId
            });

            var res = _dataContext.ExecuteReaderProcedure<bool>("TerzeParti.proc_ProvidersBookmaker_UtenteGetAbilitato", parameters: pars);

            if (res != null && res.Count() > 0)
                retVal = res.FirstOrDefault();

            return retVal;
        }
    }
}
