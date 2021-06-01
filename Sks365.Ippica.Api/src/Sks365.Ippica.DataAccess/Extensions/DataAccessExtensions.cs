using Autofac;
using Autofac.Core;
using Sks365.Ippica.Common.Config.Abstraction;
using Sks365.Ippica.Utility;

namespace Sks365.Ippica.DataAccess.Extensions
{
    public static class DataAccessExtensions
    {
        //Autofac DI container
        public static void AddUnitOfWork(this ContainerBuilder builder)
        {
            builder.Register(c => new DataContext(c.Resolve<IAppSettings>().ConnectionStrings.Mst))
                   .As<IDataContext>()
                   .Keyed<IDataContext>(DatabaseNames.Mst)
                   .InstancePerDependency();
            builder.Register(c => new DataContext(c.Resolve<IAppSettings>().ConnectionStrings.Isbets))
                   .As<IDataContext>()
                   .Keyed<IDataContext>(DatabaseNames.Isbets)
                   .InstancePerDependency();

            builder.RegisterType<IsbetsUnitOfWork>().As<IIsbetsUnitOfWork>().WithParameter(
                new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IRepositoryFactory),
                    (pi, ctx) => new RepositoryFactory(ctx.ResolveKeyed<IDataContext>(DatabaseNames.Isbets))))
                .InstancePerDependency();

            builder.RegisterType<MstUnitOfWork>().As<IMstUnitOfWork>().WithParameter(
                new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IRepositoryFactory),
                    (pi, ctx) => new RepositoryFactory(ctx.ResolveKeyed<IDataContext>(DatabaseNames.Mst))))
                .InstancePerDependency();

            //Repositories
            //builder.RegisterType<BetRepository>().As<IBetRepository>().InstancePerDependency();
            //builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerDependency();
            //builder.RegisterType<WalletRepository>().As<IWalletRepository>().InstancePerDependency();
            //builder.RegisterType<CommonRepository>().As<ICommonRepository>().InstancePerDependency();
        }
    }
}
