using Bogus;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.FakeData.Domain
{
    public class FakeBet : Bet, IFakeData<Bet>
    {
        private Faker<Bet> _fakeData;

        public FakeBet()
        {
            _fakeData = new Faker<Bet>().RuleFor(x => x.BetId, x => x.Random.Long(100000, 500000))
                                        .RuleFor(x => x.BetTypeId, x => BetTypeEnum.Fix)
                                        .RuleFor(x => x.BetStatusId, x => BetStatusEnum.Reserved)
                                        .RuleFor(x => x.Stake, x => x.Finance.Amount(0.5m, 100))
                                        .RuleFor(x => x.Amount, (x, y) => y.Stake)
                                        .RuleFor(x => x.MaxWinning, (x, y) => x.Finance.Amount((y.Stake ?? 0), (y.Stake ?? 0) + x.Random.Decimal(1, 100)))
                                        .RuleFor(x => x.Antepost, x => 0)
                                        .RuleFor(x => x.TicketId, x => x.Random.AlphaNumeric(12).ToUpper())
                                        .RuleFor(x => x.UserId, x => x.Random.Int(1000000, 5000000))
                                        .RuleFor(x => x.BetTypeId, x => BetTypeEnum.Fix)
                                        .RuleFor(x => x.CurrencyId, x => CurrencyEnum.EUR)
                                        .RuleFor(x => x.Emission, x => x.Date.Soon())
                                        .RuleFor(x => x.EmissionUtc, (x, y) => System.TimeZoneInfo.ConvertTimeToUtc(y.Emission.Value))
                                        .RuleFor(x => x.Source, x => x.Random.Int(100, 300))
                                        .RuleFor(x => x.TaxStake, x => 0)
                                        .RuleFor(x => x.TaxWin, x => 0)
                                        .RuleFor(x => x.Type, x => 0)
                                        .RuleFor(x => x.Bets, x => x.Random.Int(1, 2));
        }
        public Faker<Bet> FakeData => _fakeData;

    }
}
