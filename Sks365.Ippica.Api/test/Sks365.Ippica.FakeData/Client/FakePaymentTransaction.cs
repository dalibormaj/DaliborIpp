using Bogus;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.Payments.WebApi.Client;

namespace Sks365.Ippica.FakeData.Client
{
    public class FakePaymentTransaction : PaymentTransaction, IFakeData<PaymentTransaction>
    {
        private Faker<PaymentTransaction> _fakeData;
        public FakePaymentTransaction()
        {
            _fakeData = new Faker<PaymentTransaction>().RuleFor(x => x.Amount, x => x.Random.Decimal(1, 100000))
                                                       .RuleFor(x => x.BookmakerId, (int)BookmakerEnum.IT)
                                                       .RuleFor(x => x.CorrelationTransactionId, x => x.Random.Long(100000, 9000000))
                                                       .RuleFor(x => x.CurrencyCode, x => x.PickRandom<CurrencyEnum>().ToString("G"))
                                                       .RuleFor(x => x.ProviderId, x => (int)x.PickRandom<ProviderEnum>())
                                                       .RuleFor(x => x.State, x => x.PickRandom<PaymentStatus>())
                                                       .RuleFor(x => x.ThirdPartyId, x => x.Random.String2(10))
                                                       .RuleFor(x => x.TransactionId, x => x.Random.Long(1, 9000000))
                                                       .RuleFor(x => x.Type, x => x.PickRandom<PaymentTransactionType>())
                                                       .RuleFor(x => x.UserId, x => x.Random.Int(1000000, 9000000));
        }
        public Faker<PaymentTransaction> FakeData => _fakeData;
    }
}
