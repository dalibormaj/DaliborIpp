using Bogus;
using Sks365.Ippica.Domain.Model;

namespace Sks365.Ippica.FakeData.Domain
{
    public class FakeFixBetDetail : FixBetDetail, IFakeData<FixBetDetail>
    {
        private Faker<FixBetDetail> _fakeData;

        public FakeFixBetDetail()
        {
            _fakeData = new Faker<FixBetDetail>().RuleFor(x => x.Country, x => x.Address.CountryCode(Bogus.DataSets.Iso3166Format.Alpha3))
                                                 .RuleFor(x => x.Odd, x => x.Random.Decimal(0.01M, 25))
                                                 .RuleFor(x => x.Market, x => x.Address.City());
        }
        public Faker<FixBetDetail> FakeData => _fakeData;
    }
}
