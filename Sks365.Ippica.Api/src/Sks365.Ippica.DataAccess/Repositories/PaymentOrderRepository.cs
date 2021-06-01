using AutoMapper;
using Dapper;
using Sks365.Ippica.DataAccess.Repositories.Abstraction;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Sks365.Ippica.DataAccess.Repositories
{
    public class PaymentOrderRepository : IPaymentOrderRepository
    {
        private readonly IDataContext _dataContext;

        public PaymentOrderRepository(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public PaymentOrder GetPaymentOrder(long paymentOrderId)
        {
            var pars = new DynamicParameters(new
            {
                @switch = 0,
                IDCorrelazioneTransazioni = paymentOrderId
            });

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, PaymentOrder>()
                   .ForMember(d => d.PaymentOrderId, opt => opt.MapFrom(src => src["IDCorrelazioneTransazioni"]))
                   .ForMember(d => d.TransactionId, opt => opt.MapFrom(src => src["IDTransazioneISBets"]))
                   .ForMember(d => d.Amount, opt => opt.MapFrom(src => src["Importo"]))
                   .ForMember(d => d.CurrencyId, opt => opt.MapFrom(src => ((CurrencyEnum?)(byte?)src["IDValutaISBets"])))
                   .ForMember(d => d.StatusId, opt => opt.MapFrom(src => src["IDStato"]));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<PaymentOrder>("TerzeParti.proc_CorrelazioneTransazioni", mapper, pars).FirstOrDefault();

            return res;
        }

        public List<Transaction> GetTransactions(long paymentOrderId)
        {
            var pars = new DynamicParameters(new
            {
                IDCorrelazioneTransazioni = paymentOrderId
            });

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, Transaction>()
                   .ForMember(d => d.TransactionId, opt => opt.MapFrom(src => src["IDTransazione"]))
                   .ForMember(d => d.Amount, opt => opt.MapFrom(src => src["Importo"]))
                   .ForMember(d => d.WithdrawableAmount, opt => opt.MapFrom(src => src["ImportoPrelevabile"]))
                   .ForMember(d => d.CurrencyId, opt => opt.MapFrom(src => src["IDValuta"]))
                   .ForMember(d => d.RefundedTransactionId, opt => opt.MapFrom(src => src["IDTransazioneStorno"]));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<Transaction>("dbo.proc_GetTransactions", mapper, pars);

            return res;
        }

        public Transaction GetTransaction(long transactionId)
        {
            var pars = new DynamicParameters(new
            {
                @switch = 0,
                IDTransazione = transactionId
            });

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, Transaction>()
                   .ForMember(d => d.TransactionId, opt => opt.MapFrom(src => src["IDTransazione"]))
                   .ForMember(d => d.Amount, opt => opt.MapFrom(src => src["Importo"]))
                   .ForMember(d => d.WithdrawableAmount, opt => opt.MapFrom(src => src["ImportoPrelevabile"]))
                   .ForMember(d => d.CurrencyId, opt => opt.MapFrom(src => src["IDValuta"]));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<Transaction>("dbo.proc_Transazioni", mapper, pars).FirstOrDefault();

            return res;
        }

    }
}
