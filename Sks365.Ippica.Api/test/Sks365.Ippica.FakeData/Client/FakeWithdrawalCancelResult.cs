using Bogus;
using Sks365.Payments.WebApi.Client;

namespace Sks365.Ippica.FakeData.Client
{
    public class FakeWithdrawalCancelResult : WithdrawalCancelResult, IFakeData<WithdrawalCancelResult>
    {
        public Faker<WithdrawalCancelResult> FakeData => new Faker<WithdrawalCancelResult>()
            .RuleFor(x => x.RefundTransactionId, x => x.Random.Long(1000000));
    }
}
