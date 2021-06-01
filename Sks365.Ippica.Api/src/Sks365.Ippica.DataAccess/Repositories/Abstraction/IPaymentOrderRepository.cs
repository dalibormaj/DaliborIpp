using Sks365.Ippica.Domain.Model;
using System.Collections.Generic;

namespace Sks365.Ippica.DataAccess.Repositories.Abstraction
{
    public interface IPaymentOrderRepository : IRepository
    {
        PaymentOrder GetPaymentOrder(long paymentOrderId);
        List<Transaction> GetTransactions(long paymentOrderId);
        Transaction GetTransaction(long transactionId);
    }
}
