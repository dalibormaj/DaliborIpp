using Dapper;
using Sks365Ippica.Domain.Model;
using Sks365Ippica.Domain.Model.Enums;
using Sks365Ippica.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sks365Ippica.Repository.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly IDataContext _dataContext;

        public UserRepository(IDataContext dataContexts)
        {
            _dataContext = dataContexts;
        }

        public ICollection<SelfLimitation> GetSelfLimitations(int userId, decimal amount, LanguageEnum languageId)
        {
            var pars = new DynamicParameters(new
            {
                IDUtente = userId,
                Importo = amount,
                IDLingua = languageId
            });

            Func<IDictionary<string, object>, SelfLimitation> mapper = x => new SelfLimitation()
            {
                SelflimitationTypeId = x["IDTipoAutolimitazione"] == null ? null : (UserSelflimitationTypeEnum?)((byte?)x["IDTipoAutolimitazione"]),
                LimitAmount = x["LimitAmount"] == null ? null : (decimal?)x["LimitAmount"],
                LimitDays = x["LimitDays"] == null ? null : (int?)x["LimitDays"],
                StartDate = x["StartDate"] == null ? null : (DateTime?)x["StartDate"],
                RemainingAmount = x["RemainingAmount"] == null ? null : (decimal?)x["RemainingAmount"],
                ErrorMessage = x["ErrorMessage"] == null ? null : x["ErrorMessage"].ToString(),
                Selflimited = x["AutoLimited"] == null ? null : (bool?)x["AutoLimited"]
            };

            var res = _dataContext.ExecuteReaderProcedure("Utenti.proc_Autolimitazioni_Check", parameters: pars, mapper: mapper);

            return res;
        }

        public ICollection<User> GetUsers(int? userId = null, string userName = "", BookmakerEnum? bookmakerId = null)
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

            Func<IDictionary<string, object>, User> mapper = x => new User()
            {
                UserId = x["IDUtente"] == null ? null : (int?)x["IDUtente"],
                BookmakerId = x["IDBookmaker"] == null ? null : ((BookmakerEnum?)(short?)x["IDBookmaker"]),
                ParentId = x["IDPadre"] == null ? null : (int?)x["IDPadre"],
                Username = x["Uid"] == null ? null : x["Uid"].ToString(),
                Password = x["Pwd"] == null ? null : x["Pwd"].ToString(),
                Name = x["Nome"] == null ? null : x["Nome"].ToString(),
                Surname = x["Cognome"] == null ? null : x["Cognome"].ToString(),
                State = x["Stato"] == null ? null : (byte?)x["Stato"],
                IsTestUser = x["TestUser"] == null ? null : (byte?)x["TestUser"],
                UserTypeId = x["TipoUtente"] == null ? null : (byte?)x["TipoUtente"],
                SportWallet = new SportWallet()
                {
                    Currency = new Currency()
                    {
                        CurrencyId = x["IDValuta"] == null ? null : (CurrencyEnum?)(byte?)x["IDValuta"]
                    },
                    BookmakerId = x["IDBookmaker"] == null ? null : ((BookmakerEnum?)(short?)x["IDBookmaker"]),
                    Balance = x["Saldo"] == null ? null : (decimal?)x["Saldo"],
                    WithdrawableBalance = x["SaldoPrelevabile"] == null ? null : (decimal?)x["SaldoPrelevabile"],
                    Reservation = x["Reservation"] == null ? null : (decimal?)x["Reservation"],
                    Overdraft = x["Fido"] == null ? null : (decimal?)x["Fido"],
                },
                Description = x["Descrizione"] == null ? null : x["Descrizione"].ToString(),
                UserTypeDescription = x["TipoUtenteDesc"] == null ? null : x["TipoUtenteDesc"].ToString(),
                AccountCurrencyId = x["IDValutaConto"] == null ? null : (CurrencyEnum?)(byte)x["IDValutaConto"]
            };

            var res = _dataContext.ExecuteReaderProcedure("dbo.proc_Utenti", parameters: pars, mapper: mapper);

            return res;
        }

        public User GetUser(int userId)
        {
            return GetUsers(userId, string.Empty).FirstOrDefault();
        }

        public User GetUser(string userName, BookmakerEnum bookmakerEnum)
        {
            return GetUsers(userName: userName, bookmakerId: bookmakerEnum)?.FirstOrDefault();
        }

        public UserDetail GetUserDetails(int userId)
        {
            var pars = new DynamicParameters(new
            {
                IDUtente = userId
            });

            pars.Add(name: "@switch", dbType: System.Data.DbType.Byte, value: 0);

            Func<IDictionary<string, object>, UserDetail> mapper = x => new UserDetail()
            {
                UserId = x["IDUtente"] == null ? null : (int?)x["IDUtente"],
                Note = x["Note"] == null ? null : x["Note"].ToString(),
                ReservedNote = x["NoteRiserva"] == null ? null : x["NoteRiserva"].ToString(),
                Color = x["Colore"] == null ? null : (int?)x["Colore"],
                LastLogin = x["UltimoLogIn"] == null ? null : (DateTime?)x["UltimoLogIn"],
                CountryId = x["IDPaese"] == null ? null : (int?)x["IDPaese"],
                Address = x["Indirizzo"] == null ? null : x["Indirizzo"].ToString(),
                City = x["Citta"] == null ? null : x["Citta"].ToString(),
                PostalCode = x["Cap"] == null ? null : x["Cap"].ToString(),
                Province = x["Provincia"] == null ? null : x["Provincia"].ToString(),
                Region = x["Regione"] == null ? null : x["Regione"].ToString(),
                Telephone = x["Telefono"] == null ? null : x["Telefono"].ToString(),
                MobilePhone = x["Cellulare"] == null ? null : x["Cellulare"].ToString(),
                BirthDate = x["DataNascita"] == null ? null : (DateTime?)x["DataNascita"],
                Email = x["Email"] == null ? null : x["Email"].ToString(),
                CreationDate = x["DataCreazione"] == null ? null : (DateTime?)x["DataCreazione"],
                SecurityQuestionId = x["IDDomandaSicurezza"] == null ? null : (short?)x["IDDomandaSicurezza"],
                SecurityAnswer = x["RipostaSicurezza"] == null ? null : x["RipostaSicurezza"].ToString(),
                IDGMT = x["IDGMT"] == null ? null : (byte?)x["IDGMT"],
                InternationalCountryCodeId = x["IDPaeseInternazionaleCodice"] == null ? null : x["IDPaeseInternazionaleCodice"].ToString(),
                BetsReserveUserTypeId = x["IDTipoUtentiScommesseRiserva"] == null ? null : (int?)x["IDTipoUtentiScommesseRiserva"]
            };

            var res = _dataContext.ExecuteReaderProcedure("dbo.proc_UtentiDettaglio", parameters: pars, mapper: mapper).FirstOrDefault();

            //_logger.LogInformation("{Date} {Class} {MethodName} {@UserId} {@Result} ", DateTime.Now, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, userId, res);

            return res;
        }

        public ICollection<UserAdditionalData> GetUserAdditionalData(int userId, UserDataTypeEnum? userDataTypeId = null, string value = null)
        {
            var pars = new DynamicParameters(new
            {
                Switch = 4,
                IDUtente = userId
            });

            if (!string.IsNullOrEmpty(value))
                pars.Add(name: "@IDTipoDatoUtente", dbType: System.Data.DbType.Int32, value: userDataTypeId);
            if (!string.IsNullOrEmpty(value))
                pars.Add(name: "@Valore", dbType: System.Data.DbType.String, value: value);

            Func<IDictionary<string, object>, UserAdditionalData> mapper = x => new UserAdditionalData()
            {
                UserDataTypeId = x["IDTipoDatoUtente"] == null ? null : (UserDataTypeEnum?)(short?)x["IDTipoDatoUtente"],
                Value = x["Valore"] == null ? null : x["Valore"].ToString()
            };

            var res = _dataContext.ExecuteReaderProcedure("dbo.proc_UtentiDatiAggiuntivi", parameters: pars, mapper: mapper);

            return res;
        }

        public int UpdateUserAdditionalData(int userId, UserDataTypeEnum userDataTypeId, string value)
        {
            var pars = new DynamicParameters(new
            {
                IDUtente = userId,
                IDTipoDatoUtente = userDataTypeId,
                Valore = value
            });

            pars.Add(name: "@switch", dbType: System.Data.DbType.Byte, value: 1);

            var res = _dataContext.ExecuteProcedure("dbo.proc_UtentiDatiAggiuntivi", parameters: pars);

            return res;
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
