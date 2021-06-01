using Bogus;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.FakeData.Domain
{
    public class FakeCurrency : Currency, IFakeData<Currency>
    {
        private Faker<Currency> _fakeData;
        private readonly CurrencyEnum _randomCurrency = new Faker().PickRandom<CurrencyEnum>();
        public FakeCurrency()
        {
            _fakeData = new Faker<Currency>().RuleFor(x => x.BalanceTolerance, x => x.Random.Decimal(0, 1000000))
                                             .RuleFor(x => x.Code, _randomCurrency.ToString("G"))
                                             .RuleFor(x => x.CurrencyId, x => _randomCurrency)
                                             .RuleFor(x => x.IsoCode, _randomCurrency.ToString("G"))
                                             .RuleFor(x => x.Name, _randomCurrency.ToString("G"))
                                             .RuleFor(x => x.NumberOfDecimals, (byte)2)
                                             .RuleFor(x => x.Symbol, _randomCurrency.ToString("G"));
        }

        public Faker<Currency> FakeData => _fakeData;
    }
}
