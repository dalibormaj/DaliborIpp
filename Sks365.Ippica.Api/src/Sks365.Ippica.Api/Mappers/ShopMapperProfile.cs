using AutoMapper;
using Newtonsoft.Json;
using Sks365.Ippica.Api.Dto;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Api.Dto.Responses;
using Sks365.Ippica.Api.Utility;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sks365.Ippica.Api.Mappers
{
    public class ShopMapperProfile : Profile
    {
        public ShopMapperProfile()
        {
            MapShopReserveBet();
            MapShopPlaceBet();
            MapShopRollbackBet();
            MapShopCancelBet();
            MapShopSettleBet();
            MapShopPayBet();
            MapShopIdentification();
            MapShopKeepAlive();
        }

        public override string ProfileName
        {
            get { return Enum.GetName(typeof(MapperName), MapperName.ShopMapper); }
        }

        private void MapShopReserveBet()
        {
            CreateMap<BetDetailDto, FixBetDetail>().ForMember(d => d.Odd, opt => opt.MapFrom(src => (decimal)src.Odd / 100));
            CreateMap<BetDetailDto, SysBetDetail>().ForMember(d => d.Odd, opt => opt.MapFrom(src => (decimal)src.Odd / 100));
            CreateMap<PsipScommessaDto, PsipScommessa>().ForMember(d => d.Importo, opt => opt.MapFrom(src => (decimal)src.Importo / 100))
                                                        .ForMember(d => d.Mappa, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Mappa)));

            CreateMap<PsipBetDto, PsipBetDetail>();

            CreateMap<PsrScommessaDto, PsrScommessa>().ForMember(d => d.ImportoScommessa, opt => opt.MapFrom(src => (decimal)src.ImportoScommessa / 100));
            CreateMap<PsrScommessaGruppoDto, PsrScommessaGruppo>();
            CreateMap<PsrScommessaEventoDto, PsrScommessaEvento>().ForMember(d => d.Mappa, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Mappa)));
            CreateMap<PsrBetDto, PsrBetDetail>();

            CreateMap<ShopReserveBetRequest<BetDto>, List<BetTransaction>>().ConvertUsing(src => BetTransactionFactory.Create(src));
            CreateMap<ShopReserveBetRequest<PsipBetDto>, List<BetTransaction>>().ConvertUsing(src => BetTransactionFactory.Create(src));
            CreateMap<ShopReserveBetRequest<PsrBetDto>, List<BetTransaction>>().ConvertUsing(src => BetTransactionFactory.Create(src));

            CreateMap<ShopReserveBetRequest<BetDto>, Bet>().ConvertUsing((src, dest, context) => BetFactory.Create(src, context.Mapper));
            CreateMap<ShopReserveBetRequest<PsipBetDto>, Bet>().ConvertUsing((src, dest, context) => BetFactory.Create(src, context.Mapper));
            CreateMap<ShopReserveBetRequest<PsrBetDto>, Bet>().ConvertUsing((src, dest, context) => BetFactory.Create(src, context.Mapper));

            CreateMap<ShopReserveBetRequest<BetDto>, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));
            CreateMap<ShopReserveBetRequest<PsipBetDto>, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));
            CreateMap<ShopReserveBetRequest<PsrBetDto>, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));

            CreateMap<Bet, ShopReserveBetResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetBetReturnCode(src)))
                                                    .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetBetReturnCode(src))))
                                                    .ForMember(d => d.Transaction, opt => opt.MapFrom(src => src.BetId.ToString()))
                                                    .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));

        }

        private void MapShopPlaceBet()
        {
            CreateMap<ShopPlaceBetRequest<BetDto>, List<BetTransaction>>().ConvertUsing(x => BetTransactionFactory.Create(x));
            CreateMap<ShopPlaceBetRequest<PsipBetDto>, List<BetTransaction>>().ConvertUsing(x => BetTransactionFactory.Create(x));
            CreateMap<ShopPlaceBetRequest<PsrBetDto>, List<BetTransaction>>().ConvertUsing(x => BetTransactionFactory.Create(x));

            CreateMap<ShopPlaceBetRequest<BetDto>, Bet>().ConvertUsing((src, dest, context) => BetFactory.Create(src, context.Mapper));
            CreateMap<ShopPlaceBetRequest<PsipBetDto>, Bet>().ConvertUsing((src, dest, context) => BetFactory.Create(src, context.Mapper));
            CreateMap<ShopPlaceBetRequest<PsrBetDto>, Bet>().ConvertUsing((src, dest, context) => BetFactory.Create(src, context.Mapper));

            CreateMap<ShopPlaceBetRequest<BetDto>, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));
            CreateMap<ShopPlaceBetRequest<PsipBetDto>, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));
            CreateMap<ShopPlaceBetRequest<PsrBetDto>, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));

            CreateMap<Bet, ShopPlaceBetResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetBetReturnCode(src)))
                                                  .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetBetReturnCode(src))))
                                                  .ForMember(d => d.Transaction, opt => opt.MapFrom(src => src.BetId.ToString()))
                                                  .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }

        private void MapShopRollbackBet()
        {
            CreateMap<ShopRollbackBetRequest, List<BetTransaction>>().ConvertUsing(src => BetTransactionFactory.Create(src));
            CreateMap<ShopRollbackBetRequest, Bet>().ForMember(d => d.BetStatusId, opt => opt.MapFrom(src => BetStatusEnum.Refunded));
            CreateMap<ShopRollbackBetRequest, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));


            CreateMap<Bet, ShopRollbackBetResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetBetReturnCode(src)))
                                                     .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetBetReturnCode(src))))
                                                     .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }

        private void MapShopCancelBet()
        {
            CreateMap<ShopCancelBetRequest, List<BetTransaction>>().ConvertUsing(src => BetTransactionFactory.Create(src));
            CreateMap<ShopCancelBetRequest, Bet>().ForMember(d => d.BetStatusId, opt => opt.MapFrom(src => BetStatusEnum.Refunded))
                                                  .ForMember(d => d.ExternalId, opt => opt.MapFrom(src => src.TicketId));
            CreateMap<ShopCancelBetRequest, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));


            CreateMap<Bet, ShopCancelBetResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetBetReturnCode(src)))
                                                   .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetBetReturnCode(src))))
                                                   .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }

        private void MapShopSettleBet()
        {
            CreateMap<TicketDto, List<BetTransaction>>().ConvertUsing(src => BetTransactionFactory.Create(src));
            CreateMap<TicketDto, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));

            CreateMap<Bet, ShopSettleBetResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetBetReturnCode(src)))
                                                   .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetBetReturnCode(src))))
                                                   .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }

        private void MapShopPayBet()
        {
            CreateMap<ShopPayBetRequest, List<BetTransaction>>().ConvertUsing(src => BetTransactionFactory.Create(src));
            CreateMap<ShopPayBetRequest, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));

            CreateMap<Bet, ShopPayBetResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetBetReturnCode(src)))
                                                .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetBetReturnCode(src))))
                                                .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }

        private void MapShopIdentification()
        {
            CreateMap<User, ShopIdentificationResponse>().ForMember(d => d.MstCode, opt => opt.MapFrom(src => src.AdditionalData.Where(x => x.UserDataTypeId == UserDataTypeEnum.ExalogicCodDiritto).FirstOrDefault().Value))
                                                         .ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetIdentificationReturnCode(src)))
                                                         .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetIdentificationReturnCode(src))))
                                                         .ForMember(d => d.Language, opt => opt.MapFrom(src => src.GetUserLanguageCode()))
                                                         .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }

        private void MapShopKeepAlive()
        {
            CreateMap<User, ShopKeepAliveResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetIdentificationReturnCode(src)))
                                                    .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetIdentificationReturnCode(src))))
                                                    .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }


        private int GetUserBalanceReturnCode(User user)
        {
            return (user?.UserId ?? 0) >= 0 ? (int)ReturnCodeEnum.OK : (int)ReturnCodeEnum.UserNotFound;
        }

        private int GetBetReturnCode(Bet bet)
        {
            return (bet?.BetStatusId ?? 0) != BetStatusEnum.Error ? (int)ReturnCodeEnum.OK : (int)ReturnCodeEnum.Error;
        }

        private int GetIdentificationReturnCode(User user)
        {
            return (user?.UserId ?? 0) >= 0 ? (int)ReturnCodeEnum.OK : (int)ReturnCodeEnum.UserNotFound;
        }


        private string GetReturnCodeDesc(int returnCode)
        {
            return returnCode == (int)ReturnCodeEnum.OK ? "Success" : "Error";
        }
    }
}


