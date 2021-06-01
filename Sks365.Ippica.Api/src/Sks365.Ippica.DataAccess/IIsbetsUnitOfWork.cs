using Sks365.Ippica.DataAccess.Repositories.Abstraction;

namespace Sks365.Ippica.DataAccess
{
    public interface IIsbetsUnitOfWork : IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        IWalletRepository WalletRepository { get; }
        ICommonRepository CommonRepository { get; }
        IPaymentOrderRepository PaymentOrderRepository { get; }
    }
}
