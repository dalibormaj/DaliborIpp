using Bogus;
using Sks365.Payments.WebApi.Client;

namespace Sks365.Ippica.FakeData.Client
{
    public class FakeDepositRefundResult : DepositRefundResult, IFakeData<DepositRefundResult>
    {
        public Faker<DepositRefundResult> FakeData => new Faker<DepositRefundResult>()
            .RuleFor(x => x.RefundTransactionId, x => x.Random.Long(1000000));
    }
}
