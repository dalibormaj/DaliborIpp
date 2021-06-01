using Bogus;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.FakeData.Domain
{
    public class FakeSportWallet : SportWallet, IFakeData<SportWallet>
    {
        private Faker<SportWallet> _fakeData;
        public FakeSportWallet()
        {
            _fakeData = new Faker<SportWallet>().RuleFor(x => x.Balance, x => x.Finance.Amount(3000, 12000))
                                                .RuleFor(x => x.Currency, x => new Currency() { Code = "EUR", CurrencyId = CurrencyEnum.EUR })
                                                .RuleFor(x => x.WithdrawableBalance, (x, y) => y.Balance);
        }
        public Faker<SportWallet> FakeData => _fakeData;
    }
}
