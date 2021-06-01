using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sks365.Ippica.Common.Utility;
using Sks365.SessionTracker.Client.Configuration;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Sks365.SessionTracker.Client
{
    public static class SessionTrackerExtensions
    {
        //Autofac DI container
        public static void AddSessionTracker(this ContainerBuilder builder, IConfiguration configurations)
        {
            var settings = new SessionTrackerSettings();
            configurations.GetSection(typeof(SessionTrackerSettings).Name).Bind(settings);
            builder.Register(x => settings).SingleInstance();

            var options = new ConfigurationOptions();
            options.EndPoints.Add(settings.Host, settings.Port);
            options.ConnectTimeout = settings.ConnectTimeout;
            options.SyncTimeout = settings.SyncTimeout;
            options.AllowAdmin = settings.AllowAdmin;
            options.ClientName = settings.ClientName;
            options.CommandMap = CommandMap.Create(new HashSet<string> { "SUBSCRIBE" }, false);//disables creating additional (pub/sub) connection

            builder.Register(c => new RedisConnector(options))
                   .As<IRedisConnector>()
                   .Keyed<IRedisConnector>(typeof(SessionTracker).Name)
                   .SingleInstance();

            builder.Register(x => new SessionTracker(x.Resolve<ILogger<SessionTracker>>(),
                                                     x.ResolveKeyed<IRedisConnector>(typeof(SessionTracker).Name),
                                                     settings))
                   .As<ISessionTracker>().InstancePerDependency();
        }
    }
}
