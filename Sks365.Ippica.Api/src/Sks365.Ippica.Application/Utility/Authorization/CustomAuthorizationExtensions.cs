using Autofac;

namespace Sks365.Ippica.Application.Utility.Authorization
{
    public static class CustomAuthorizationExtensions
    {
        //Autofac DI container
        public static void AddCustomAuthorization(this ContainerBuilder builder)
        {
            builder.RegisterType<CustomAuthorization>().As<ICustomAuthorization>().InstancePerDependency();
        }
    }
}
