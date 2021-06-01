using Bogus;
using Sks365.Ippica.Domain.Model;

namespace Sks365.Ippica.FakeData.Domain
{
    public class FakeWebBetRequest : WebBetRequest, IFakeData<WebBetRequest>
    {
        private Faker<WebBetRequest> _fakeData;

        public FakeWebBetRequest(BetRequest betRequest)
        {
            _fakeData = new Faker<WebBetRequest>().RuleFor(x => x.BetRequestId, x => betRequest?.BetRequestId)
                                                  .RuleFor(x => x.UserAccount, x => betRequest?.UserId?.ToString())
                                                  .RuleFor(x => x.BetSettlementId, x => x.Random.Long(100000, 500000));
        }
        public Faker<WebBetRequest> FakeData => _fakeData;
    }
}
