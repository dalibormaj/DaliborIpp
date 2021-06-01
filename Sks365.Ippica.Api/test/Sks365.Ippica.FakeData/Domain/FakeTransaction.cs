using Bogus;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.FakeData.Domain
{
    public class FakeTransaction : Transaction, IFakeData<Transaction>
    {
        public Faker<Transaction> FakeData => new Faker<Transaction>()
            .RuleFor(x => x.TransactionId, x => x.Random.Long(1000000))
            .RuleFor(x => x.RefundedTransactionId, x => x.Random.Long(1000000))
            .RuleFor(x => x.CurrencyId, x => x.PickRandom<CurrencyEnum>())
            .RuleFor(x => x.Amount, x => x.Random.Decimal(1, 100000))
            .RuleFor(x => x.WithdrawableAmount, (x, t) => x.Random.Decimal(0, t.Amount.Value));
    }
}
