using Bogus;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.FakeData.Domain
{
    public class FakeBonusWallet : BonusWallet, IFakeData<BonusWallet>
    {
        private Faker<BonusWallet> _fakeData;

        public FakeBonusWallet()
        {
            _fakeData = new Faker<BonusWallet>().RuleFor(x => x.Balance, x => x.Random.Decimal(0, 10000))
                                                .RuleFor(x => x.BonusStatusId, x => (byte)x.Random.Int(0, byte.MaxValue))
                                                .RuleFor(x => x.BookmakerId, x => x.PickRandom<BookmakerEnum>())
                                                .RuleFor(x => x.CampaignId, x => x.Random.Int(1, 1000000))
                                                .RuleFor(x => x.Currency, new FakeCurrency().FakeData.Generate())
                                                .RuleFor(x => x.UserId, x => x.Random.Int(1000000, 5000000));
        }

        public Faker<BonusWallet> FakeData => _fakeData;
    }
}
