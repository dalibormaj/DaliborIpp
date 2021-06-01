using Sks365.Ippica.Domain.Model;

namespace Sks365.Ippica.DataAccess.Repositories.Abstraction
{
    public interface IWalletRepository : IRepository
    {
        SportWallet GetSportWallet(int userId);
        BonusWallet GetBonusWallet(int userId);
        Currency GetCurrency(int currencyId);
    }
}
