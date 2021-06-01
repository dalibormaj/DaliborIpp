using Bogus;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System;

namespace Sks365.Ippica.FakeData.Domain
{
    public class FakeBetTransaction : BetTransaction, IFakeData<BetTransaction>
    {
        private Faker<BetTransaction> _fakeData;
        private BetTransactionTypeEnum _tranTypeId;
        public FakeBetTransaction(BetTransactionTypeEnum tranTypeId)
        {
            _tranTypeId = tranTypeId;
            _fakeData = new Faker<BetTransaction>()
                  .RuleFor(x => x.BetTransactionTypeId, _tranTypeId)
                  .RuleFor(x => x.Amount, x =>
                  {
                      var amount = 0m;
                      if (_tranTypeId == BetTransactionTypeEnum.Stake ||
                          _tranTypeId == BetTransactionTypeEnum.TaxStake ||
                          _tranTypeId == BetTransactionTypeEnum.RefundStakeCompensation ||
                          _tranTypeId == BetTransactionTypeEnum.RefundWin ||
                          _tranTypeId == BetTransactionTypeEnum.RefundTaxWin)
                      {
                          amount = x.Finance.Amount(-10, -150);
                      }
                      else if (_tranTypeId == BetTransactionTypeEnum.Win ||
                               _tranTypeId == BetTransactionTypeEnum.TaxWin ||
                               _tranTypeId == BetTransactionTypeEnum.StakeCompensation ||
                               _tranTypeId == BetTransactionTypeEnum.RefundStake ||
                               _tranTypeId == BetTransactionTypeEnum.RefundTaxStake)
                      {
                          amount = x.Finance.Amount(10, 150);
                      }
                      return amount;
                  })
                 .RuleFor(x => x.CurrencyCode, Enum.GetName(typeof(CurrencyEnum), CurrencyEnum.EUR));
        }

        public Faker<BetTransaction> FakeData => _fakeData;


    }
}
