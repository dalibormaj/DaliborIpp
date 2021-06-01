using Autofac;
using Sks365.Ippica.Application.Services.Abstraction;

namespace Sks365.Ippica.Application.Services
{
    public static class ServiceExtensions
    {
        //Autofac DI container
        public static void AddServices(this ContainerBuilder builder)
        {
            builder.RegisterType<UserService>().As<IUserService>().InstancePerDependency();
            builder.RegisterType<BetService>().As<IBetService>().InstancePerDependency();
            builder.RegisterType<PaymentOrderService>().As<IPaymentOrderService>().InstancePerDependency();
        }
    }
}
