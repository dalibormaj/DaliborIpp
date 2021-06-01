using Bogus;
using Sks365.Ippica.Domain.Model;

namespace Sks365.Ippica.FakeData.Domain
{
    public class FakeBetRequest : BetRequest, IFakeData<BetRequest>
    {
        private Faker<BetRequest> _fakeData;

        public FakeBetRequest()
        {
            _fakeData = new Faker<BetRequest>().RuleFor(x => x.Session, x => x.Random.AlphaNumeric(80).ToUpper())
                                               .RuleFor(x => x.Ip, x => x.Internet.Ip());
        }
        public Faker<BetRequest> FakeData => _fakeData;
    }
}
