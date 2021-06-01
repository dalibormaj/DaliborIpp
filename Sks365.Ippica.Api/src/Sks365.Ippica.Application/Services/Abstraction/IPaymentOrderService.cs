using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System.Threading.Tasks;

namespace Sks365.Ippica.Application.Services.Abstraction
{
    public interface IPaymentOrderService
    {
        PaymentOrder GetPaymentOrder(long paymentOrderId);
        Task<PaymentOrder> Initiate(decimal amount, CurrencyEnum currency, int userId, BookmakerEnum bookmakerId, string ip);
        Task<PaymentOrder> Settle(long paymentOrderId, string externalId, int userId, BookmakerEnum bookmakerId, string ip);
        Task<PaymentOrder> SettleStakeCompensation(long paymentOrderId, string externalId, int userId, BookmakerEnum bookmakerId, string ip);
        Task<PaymentOrder> Refund(long paymentOrderId, int userId, BookmakerEnum bookmakerId, string ip);
        Task<PaymentOrder> UpdateThirdPartyId(long paymentOrderId, string thirdPartyId, int userId, BookmakerEnum bookmakerId, string ip);
        Task<BetTransaction> ProcessTransaction(BetTransaction transaction, int userId, BookmakerEnum bookmakerId, string ip, bool processAsPending = false);
    }
}
