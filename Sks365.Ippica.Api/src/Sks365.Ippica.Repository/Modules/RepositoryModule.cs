using Autofac;
using Autofac.Core;
using Sks365Ippica.Common.Config.Abstraction;
using Sks365Ippica.Repository.Abstraction;

namespace Sks365Ippica.Repository.Modules
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new DataContext(c.Resolve<IAppSettings>().ConnectionStrings.Find(x => x.Name.ToLower().Equals(DatabaseNamesIppica.ToLower())).ConnectionString))
                   .As<IDataContext>()
                   .Keyed<IDataContext>(DatabaseNames.Mst)
                   .InstancePerDependency();
            builder.Register(c => new DataContext(c.Resolve<IAppSettings>().ConnectionStrings.Find(x => x.Name.ToLower().Equals(DatabaseNames.Isbets.ToLower())).ConnectionString))
                   .As<IDataContext>()
                   .Keyed<IDataContext>(DatabaseNames.Isbets)
                   .InstancePerDependency();

            builder.RegisterType<IsbetsUnitOfWork>().As<IIsbetsUnitOfWork>().WithParameter(
                new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IDataContext),
                    (pi, ctx) => ctx.ResolveKeyed<IDataContext>(DatabaseNames.Isbets)))
                .InstancePerDependency();

            builder.RegisterType<MstUnitOfWork>().As<IMstUnitOfWork>().WithParameter(
                new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IDataContext),
                    (pi, ctx) => ctx.ResolveKeyed<IDataContext>(DatabaseNames.Mst)))
                .InstancePerDependency();

            //Repositories
            //builder.RegisterType<BetRepository>().As<IBetRepository>().InstancePerDependency();
            //builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerDependency();
            //builder.RegisterType<WalletRepository>().As<IWalletRepository>().InstancePerDependency();
            //builder.RegisterType<CommonRepository>().As<ICommonRepository>().InstancePerDependency();
        }
    }
}
