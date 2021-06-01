using Autofac;
using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Sks365.Ippica.Api.Dto;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Api.Validators;
using Sks365.Ippica.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sks365.Ippica.Api.Extensions
{
    /// <summary>
    /// Extension methods on Autofac ContainerBuilder
    /// </summary>
    public static class ContainerBuilderExtensions
    {
        public static void AddAppSettings(this ContainerBuilder builder, IConfiguration configuration)
        {
            builder.Register(x => configuration.GetAppSettings()).SingleInstance();
        }

        public static void AddValidators(this ContainerBuilder builder)
        {
            builder.RegisterType<GlobalValidator>().As<IGlobalValidator>();

            //dto validators
            builder.RegisterType<ShopReserveBetValidator>().As<IValidator<ShopReserveBetRequest<BetDto>>>().InstancePerDependency();
            builder.RegisterType<ShopReserveBetPsipValidator>().As<IValidator<ShopReserveBetRequest<PsipBetDto>>>().InstancePerDependency();
            builder.RegisterType<ShopReserveBetPsrValidator>().As<IValidator<ShopReserveBetRequest<PsrBetDto>>>().InstancePerDependency();
            builder.RegisterType<ShopPlaceBetValidator>().As<IValidator<ShopPlaceBetRequest<BetDto>>>().InstancePerDependency();
            builder.RegisterType<ShopPlaceBetPsipValidator>().As<IValidator<ShopPlaceBetRequest<PsipBetDto>>>().InstancePerDependency();
            builder.RegisterType<ShopPlaceBetPsrValidator>().As<IValidator<ShopPlaceBetRequest<PsrBetDto>>>().InstancePerDependency();
            builder.RegisterType<ShopIdentificationValidator>().As<IValidator<ShopIdentificationRequest>>().InstancePerDependency();
            builder.RegisterType<ShopKeepAliveValidator>().As<IValidator<ShopKeepAliveRequest>>().InstancePerDependency();
            builder.RegisterType<ShopRollbackBetValidator>().As<IValidator<ShopRollbackBetRequest>>().InstancePerDependency();
            builder.RegisterType<ShopCancelBetValidator>().As<IValidator<ShopCancelBetRequest>>().InstancePerDependency();
            builder.RegisterType<ShopSettleBetValidator>().As<IValidator<ShopSettleBetRequest>>().InstancePerDependency();
            builder.RegisterType<ShopPayBetValidator>().As<IValidator<ShopPayBetRequest>>().InstancePerDependency();

            builder.RegisterType<WebReserveBetValidator>().As<IValidator<WebReserveBetRequest<BetDto>>>().InstancePerDependency();
            builder.RegisterType<WebReserveBetPsipValidator>().As<IValidator<WebReserveBetRequest<PsipBetDto>>>().InstancePerDependency();
            builder.RegisterType<WebReserveBetPsrValidator>().As<IValidator<WebReserveBetRequest<PsrBetDto>>>().InstancePerDependency();
            builder.RegisterType<WebPlaceBetValidator>().As<IValidator<WebPlaceBetRequest<BetDto>>>().InstancePerDependency();
            builder.RegisterType<WebPlaceBetPsipValidator>().As<IValidator<WebPlaceBetRequest<PsipBetDto>>>().InstancePerDependency();
            builder.RegisterType<WebPlaceBetPsrValidator>().As<IValidator<WebPlaceBetRequest<PsrBetDto>>>().InstancePerDependency();
            builder.RegisterType<WebIdentificationValidator>().As<IValidator<WebIdentificationRequest>>().InstancePerDependency();
            builder.RegisterType<WebKeepAliveValidator>().As<IValidator<WebKeepAliveRequest>>().InstancePerDependency();
            builder.RegisterType<WebRollbackBetValidator>().As<IValidator<WebRollbackBetRequest>>().InstancePerDependency();
            builder.RegisterType<WebSettleBetValidator>().As<IValidator<WebSettleBetRequest>>().InstancePerDependency();
            builder.RegisterType<WebUserBalanceValidator>().As<IValidator<WebUserBalanceRequest>>().InstancePerDependency();
        }

        public static void AddMappers(this ContainerBuilder builder)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.FullName
                             .Substring(0, 13)
                             .Equals("Sks365.Ippica", StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .ToArray();

            builder.RegisterAssemblyTypes(assemblies)
                   .Where(t => typeof(Profile).IsAssignableFrom(t) && !t.IsAbstract && t.IsPublic)
                   .As<Profile>();

            //Mapper locator
            builder.Register(c =>
            {
                Dictionary<MapperName, IMapper> mapperList = new Dictionary<MapperName, IMapper>();
                foreach (var profile in c.Resolve<IEnumerable<Profile>>())
                {
                    MapperName mapperName = MapperName.NotDefined;
                    var config = new MapperConfiguration(cfg =>
                    {
                        mapperName = (MapperName)Enum.Parse(typeof(MapperName), profile.ProfileName);
                        cfg.AddProfile(profile);
                    });
                    var mapper = config.CreateMapper();
                    mapperList.Add(mapperName, mapper);
                }
                return new MapperLocator(mapperList);
            }
            ).As<IMapperLocator>().SingleInstance();
        }
    }
}