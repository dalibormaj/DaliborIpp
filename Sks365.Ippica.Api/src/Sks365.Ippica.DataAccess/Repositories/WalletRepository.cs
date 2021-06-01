using AutoMapper;
using Dapper;
using Sks365.Ippica.DataAccess.Repositories.Abstraction;
using Sks365.Ippica.Domain.Model;
using System.Collections.Generic;
using System.Linq;

namespace Sks365.Ippica.DataAccess.Repositories
{
    public class WalletRepository : IWalletRepository
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

            var currencyMapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, Currency>()
                   .ForMember(d => d.CurrencyId, opt => opt.MapFrom(src => src["IDValuta"]))
                   .ForMember(d => d.Name, opt => opt.MapFrom(src => src["Valuta"]))
                   .ForMember(d => d.Code, opt => opt.MapFrom(src => src["Codice"]))
                   .ForMember(d => d.Symbol, opt => opt.MapFrom(src => src["Simbolo"]));
            }).CreateMapper();

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, BonusWallet>()
                   .ForMember(d => d.UserId, opt => opt.MapFrom(src => src["IDUtente"]))
                   .ForMember(d => d.CampaignId, opt => opt.MapFrom(src => src["IDCampagna"]))
                   .ForMember(d => d.Balance, opt => opt.MapFrom(src => src["Saldo"]))
                   .ForMember(d => d.BonusStatusId, opt => opt.MapFrom(src => src["IDStatoBonus"]))
                   .ForMember(d => d.Currency, opt => opt.MapFrom(src => currencyMapper.Map<Currency>(src)));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<BonusWallet>("Bonus.GetBonusBalance", mapper, parameters: pars).FirstOrDefault();
            return res;
        }

        public Currency GetCurrency(int currencyId)
        {
            var pars = new DynamicParameters(new
            {
                @Switch = 0,
                IDValuta = currencyId
            });
            //pars.Add(name: "@switch", dbType: DbType.Byte, value: 0);

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, Currency>()
                   .ForMember(d => d.CurrencyId, opt => opt.MapFrom(src => src["IDValuta"]))
                   .ForMember(d => d.Name, opt => opt.MapFrom(src => src["Valuta"]))
                   .ForMember(d => d.Code, opt => opt.MapFrom(src => src["Codice"]))
                   .ForMember(d => d.Symbol, opt => opt.MapFrom(src => src["Simbolo"]))
                   .ForMember(d => d.IsoCode, opt => opt.MapFrom(src => src["CodiceIso"]))
                   .ForMember(d => d.NumberOfDecimals, opt => opt.MapFrom(src => src["NumeroDecimali"]))
                   .ForMember(d => d.BalanceTolerance, opt => opt.MapFrom(src => src["TolleranzaSaldo"]));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<Currency>("Contabilita.proc_Valute", mapper, parameters: pars);

            return res.FirstOrDefault();
        }

        public SportWallet GetSportWallet(int userId)
        {
            var pars = new DynamicParameters(new
            {
                IDUtente = userId
            });

            var currencyMapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, Currency>()
                   .ForMember(d => d.CurrencyId, opt => opt.MapFrom(src => src["IDValuta"]))
                   .ForMember(d => d.Name, opt => opt.MapFrom(src => src["Valuta"]))
                   .ForMember(d => d.Code, opt => opt.MapFrom(src => src["Codice"]))
                   .ForMember(d => d.Symbol, opt => opt.MapFrom(src => src["Simbolo"]));
            }).CreateMapper();

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, SportWallet>()
                   .ForMember(d => d.UserId, opt => opt.MapFrom(src => src["IDUtente"]))
                   .ForMember(d => d.BookmakerId, opt => opt.MapFrom(src => src["IDBookmaker"]))
                   .ForMember(d => d.Balance, opt => opt.MapFrom(src => src["Saldo"]))
                   .ForMember(d => d.WithdrawableBalance, opt => opt.MapFrom(src => src["SaldoPrelevabile"]))
                   .ForMember(d => d.Reservation, opt => opt.MapFrom(src => src["Reservation"]))
                   .ForMember(d => d.Overdraft, opt => opt.MapFrom(src => src["Fido"]))
                   .ForMember(d => d.CreationDate, opt => opt.MapFrom(src => src["DataApertura"]))
                   .ForMember(d => d.LastModificationDate, opt => opt.MapFrom(src => src["DataUltimoMov"]))
                   .ForMember(d => d.Currency, opt => opt.MapFrom(src => currencyMapper.Map<Currency>(src)));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<SportWallet>("dbo.proc_ContiVirtualiDetails", mapper, parameters: pars).FirstOrDefault();
            return res;
        }

    }
}
