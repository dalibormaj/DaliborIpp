using Autofac;
using Microsoft.Extensions.Configuration;
using Sks365.Ippica.Common.Config;
using Sks365.Ippica.Common.Utility;
using StackExchange.Redis;
using System.Collections.Generic;

namespace Sks365.Ippica.Application.Utility.OperationRecorder
{
    public static class OperationRecoredExtensions
    {
        //Autofac DI container
        public static void AddOperationRecorder(this ContainerBuilder builder, IConfiguration configurations)
        {
            var appSettings = new AppSettings();
            configurations.GetSection(typeof(AppSettings).Name).Bind(appSettings);

            var options = new ConfigurationOptions();
            options.EndPoints.Add(appSettings.OperationRecorder.Host, appSettings.OperationRecorder.Port);
            options.ConnectTimeout = appSettings.OperationRecorder.ConnectTimeout;
            options.SyncTimeout = appSettings.OperationRecorder.SyncTimeout;
            options.AllowAdmin = appSettings.OperationRecorder.AllowAdmin;
            options.ClientName = appSettings.OperationRecorder.ClientName;
            options.CommandMap = CommandMap.Create(new HashSet<string> { "SUBSCRIBE" }, false);//disables creating additional (pub/sub) connection

            builder.Register(c => new RedisConnector(options))
                   .As<IRedisConnector>()
                   .Keyed<IRedisConnector>(typeof(OperationRecorder).Name)
                   .SingleInstance();

            builder.Register(x => new OperationRecorder(x.ResolveKeyed<IRedisConnector>(typeof(OperationRecorder).Name),
                                                        appSettings.OperationRecorder.DatabaseId))
                   .As<IOperationRecorder>().InstancePerLifetimeScope();
        }
    }
}
