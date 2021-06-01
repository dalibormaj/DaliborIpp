using Autofac;

namespace Sks365.Ippica.Application.Utility.EmailSender
{
    public static class EmailSenderExtensions
    {
        //Autofac DI container
        public static void AddEmailSender(this ContainerBuilder builder)
        {
            builder.RegisterType<EmailSender>().As<IEmailSender>().InstancePerDependency();
        }
    }
}
