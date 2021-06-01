using Bogus;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.FakeData.Domain
{
    public class FakePaymentOrder : PaymentOrder, IFakeData<PaymentOrder>
    {
        private Faker<PaymentOrder> _fakeData;
        private PaymentOrderStatusEnum _statusId;
        public FakePaymentOrder()
        {
            _fakeData = new Faker<PaymentOrder>().RuleFor(x => x.PaymentOrderId, x => x.Random.Long(1, long.MaxValue))
                                                 .RuleFor(x => x.TransactionId, x => x.Random.Long(1, long.MaxValue))
                                                 .RuleFor(x => x.StatusId, x => x.PickRandom<PaymentOrderStatusEnum>())
                                                 .RuleFor(x => x.CurrencyId, x => x.PickRandom<CurrencyEnum>())
                                                 .RuleFor(x => x.Amount, x => x.Random.Decimal(0, 100000));
        }

        public Faker<PaymentOrder> FakeData => _fakeData;
    }
}
