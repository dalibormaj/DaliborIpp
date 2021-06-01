using Dapper;
using Sks365Ippica.Domain.Model;
using Sks365Ippica.Domain.Model.Enums;
using Sks365Ippica.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sks365Ippica.Repository.Repositories
{
    internal class WalletRepository : IWalletRepository
    {
        private readonly IDataContext _dataContext;

        public WalletRepository(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public BonusWallet GetBonusWallet(int userId)
        {
            var pars = new DynamicParameters(new
            {
                IDUtente = userId
            });

            Func<IDictionary<string, object>, BonusWallet> mapper = x => new BonusWallet()
            {
                UserId = x["IDUtente"] == null ? null : (int?)x["IDUtente"],
                CampaignId = x["IDCampagna"] == null ? null : (int?)x["IDCampagna"],
                Balance = x["Saldo"] == null ? null : (decimal?)x["Saldo"],
                BonusStatusId = x["IDStatoBonus"] == null ? null : (byte?)x["IDStatoBonus"],
                Currency = new Currency
                {
                    CurrencyId = x["IDValuta"] == null ? null : (CurrencyEnum?)(byte?)x["IDValuta"],
                    Name = x["Valuta"] == null ? null : x["Valuta"].ToString(),
                    Code = x["Codice"] == null ? null : x["Codice"].ToString(),
                    Symbol = x["Simbolo"] == null ? null : x["Simbolo"].ToString()
                }
            };

            var res = _dataContext.ExecuteReaderProcedure("Bonus.proc_Campagne_FindCampagnaAttivaXUtente", parameters: pars, mapper: mapper).FirstOrDefault();
            return res;
        }

        public Currency GetCurrency(int currencyId)
        {
            var pars = new DynamicParameters(new
            {
                IDValuta = currencyId
            });
            pars.Add(name: "@switch", dbType: DbType.Byte, value: 0);

            Func<IDictionary<string, object>, Currency> mapper = x => new Currency()
            {
                CurrencyId = x["IDValuta"] == null ? null : (CurrencyEnum?)(byte?)x["IDValuta"],
                Code = x["Codice"] == null ? null : x["Codice"].ToString(),
                Name = x["Valuta"] == null ? null : x["Valuta"].ToString(),
                Symbol = x["Simbolo"] == null ? null : x["Simbolo"].ToString(),
                IsoCode = x["CodiceIso"] == null ? null : x["CodiceIso"].ToString(),
                NumberOfDecimals = x["NumeroDecimali"] == null ? null : (byte?)x["NumeroDecimali"],
                BalanceTolerance = x["TolleranzaSaldo"] == null ? null : (decimal?)x["TolleranzaSaldo"]
            };

            var res = _dataContext.ExecuteReaderProcedure("Contabilita.proc_Valute", parameters: pars, mapper: mapper);

            return res.FirstOrDefault();
        }

        public SportWallet GetSportWallet(int userId)
        {
            var pars = new DynamicParameters(new
            {
                IDUtente = userId
            });

            Func<IDictionary<string, object>, SportWallet> mapper = x => new SportWallet()
            {
                UserId = x["IDUtente"] == null ? null : (int?)x["IDUtente"],
                BookmakerId = x["IDBookmaker"] == null ? null : ((BookmakerEnum?)(short?)x["IDBookmaker"]),
                Balance = x["Saldo"] == null ? null : (decimal?)x["Saldo"],
                WithdrawableBalance = x["SaldoPrelevabile"] == null ? null : (decimal?)x["SaldoPrelevabile"],
                Reservation = x["Reservation"] == null ? null : (decimal?)x["Reservation"],
                Overdraft = x["Fido"] == null ? null : (decimal?)x["Fido"],
                CreationDate = x["DataApertura"] == null ? null : (DateTime?)x["DataApertura"],
                LastModificationDate = x["DataUltimoMov"] == null ? null : (DateTime?)x["DataUltimoMov"],
                Currency = new Currency
                {
                    CurrencyId = x["IDValuta"] == null ? null : (CurrencyEnum?)(byte?)x["IDValuta"],
                    Name = x["Valuta"] == null ? null : x["Valuta"].ToString(),
                    Code = x["Codice"] == null ? null : x["Codice"].ToString(),
                    Symbol = x["Simbolo"] == null ? null : x["Simbolo"].ToString()
                }
            };

            var res = _dataContext.ExecuteReaderProcedure("dbo.proc_ContiVirtualiDetails", parameters: pars, mapper: mapper).FirstOrDefault();
            return res;
        }
    }
}
