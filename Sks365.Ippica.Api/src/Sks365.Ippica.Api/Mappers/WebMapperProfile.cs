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

namespace Sks365.Ippica.Api.Mappers
{
    public class WebMapperProfile : Profile
    {
        public WebMapperProfile()
        {
            MapWebReserveBet();
            MapWebPlaceBet();
            MapWebRollbackBet();
            MapWebSettleBet();
            MapWebIdentification();
            MapWebUserBalance();
            MapWebKeepAlive();
        }

        public override string ProfileName
        {
            get { return Enum.GetName(typeof(MapperName), MapperName.WebMapper); }
        }

        private void MapWebReserveBet()
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

            CreateMap<WebReserveBetRequest<BetDto>, List<BetTransaction>>().ConvertUsing(src => BetTransactionFactory.Create(src));
            CreateMap<WebReserveBetRequest<PsipBetDto>, List<BetTransaction>>().ConvertUsing(src => BetTransactionFactory.Create(src));
            CreateMap<WebReserveBetRequest<PsrBetDto>, List<BetTransaction>>().ConvertUsing(src => BetTransactionFactory.Create(src));

            CreateMap<WebReserveBetRequest<BetDto>, Bet>().ConvertUsing((src, dest, context) => BetFactory.Create(src, context.Mapper));
            CreateMap<WebReserveBetRequest<PsipBetDto>, Bet>().ConvertUsing((src, dest, context) => BetFactory.Create(src, context.Mapper));
            CreateMap<WebReserveBetRequest<PsrBetDto>, Bet>().ConvertUsing((src, dest, context) => BetFactory.Create(src, context.Mapper));

            CreateMap<WebReserveBetRequest<BetDto>, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));
            CreateMap<WebReserveBetRequest<PsipBetDto>, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));
            CreateMap<WebReserveBetRequest<PsrBetDto>, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));

            CreateMap<Bet, WebReserveBetResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetBetReturnCode(src)))
                                                   .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetBetReturnCode(src))))
                                                   .ForMember(d => d.Transaction, opt => opt.MapFrom(src => src.BetId.ToString()))
                                                   .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }

        private void MapWebPlaceBet()
        {
            CreateMap<WebPlaceBetRequest<BetDto>, List<BetTransaction>>().ConvertUsing(x => BetTransactionFactory.Create(x));
            CreateMap<WebPlaceBetRequest<PsipBetDto>, List<BetTransaction>>().ConvertUsing(x => BetTransactionFactory.Create(x));
            CreateMap<WebPlaceBetRequest<PsrBetDto>, List<BetTransaction>>().ConvertUsing(x => BetTransactionFactory.Create(x));

            CreateMap<WebPlaceBetRequest<BetDto>, Bet>().ConvertUsing((src, dest, context) => BetFactory.Create(src, context.Mapper));
            CreateMap<WebPlaceBetRequest<PsipBetDto>, Bet>().ConvertUsing((src, dest, context) => BetFactory.Create(src, context.Mapper));
            CreateMap<WebPlaceBetRequest<PsrBetDto>, Bet>().ConvertUsing((src, dest, context) => BetFactory.Create(src, context.Mapper));

            CreateMap<WebPlaceBetRequest<BetDto>, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));
            CreateMap<WebPlaceBetRequest<PsipBetDto>, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));
            CreateMap<WebPlaceBetRequest<PsrBetDto>, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));

            CreateMap<Bet, WebPlaceBetResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetBetReturnCode(src)))
                                                 .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetBetReturnCode(src))))
                                                 .ForMember(d => d.Transaction, opt => opt.MapFrom(src => src.BetId.ToString()))
                                                 .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }

        private void MapWebRollbackBet()
        {
            CreateMap<WebRollbackBetRequest, List<BetTransaction>>().ConvertUsing(src => BetTransactionFactory.Create(src));
            CreateMap<WebRollbackBetRequest, Bet>().ForMember(d => d.BetStatusId, opt => opt.MapFrom(src => BetStatusEnum.Refunded));
            CreateMap<WebRollbackBetRequest, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));


            CreateMap<Bet, WebRollbackBetResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetBetReturnCode(src)))
                                                    .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetBetReturnCode(src))))
                                                    .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }

        private void MapWebSettleBet()
        {
            CreateMap<WebSettleBetRequest, List<BetTransaction>>().ConvertUsing(src => BetTransactionFactory.Create(src));
            CreateMap<WebSettleBetRequest, BetRequest>().ConvertUsing((src, dest, context) => BetRequestFactory.Create(src));

            CreateMap<Bet, WebSettleBetResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetBetReturnCode(src)))
                                                  .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetBetReturnCode(src))))
                                                  .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }

        private void MapWebIdentification()
        {
            CreateMap<User, WebIdentificationResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetIdentificationReturnCode(src)))
                                                        .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetIdentificationReturnCode(src))))
                                                        .ForMember(d => d.Session, opt => opt.MapFrom(src => string.Empty))
                                                        .ForMember(d => d.Language, opt => opt.MapFrom(src => src.GetUserLanguageCode()))
                                                        .ForMember(d => d.UserAccount, opt => opt.MapFrom(src => src.UserId))
                                                        .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }
        private void MapWebUserBalance()
        {
            CreateMap<User, WebUserBalanceResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetUserBalanceReturnCode(src)))
                                                     .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetUserBalanceReturnCode(src))))
                                                     .ForMember(d => d.Balance, opt => opt.MapFrom(src => (src.SportWallet.Balance ?? 0) * 100))
                                                     .ForMember(d => d.Currency, opt => opt.MapFrom(src => Enum.GetName(typeof(CurrencyEnum), src.SportWallet.Currency.CurrencyId)))
                                                     .ForMember(d => d.Language, opt => opt.MapFrom(src => src.GetUserLanguageCode()))
                                                     .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => DateTime.Now.ToMicrosoftDate()));
        }

        private void MapWebKeepAlive()
        {
            CreateMap<User, WebKeepAliveResponse>().ForMember(d => d.ReturnCode, opt => opt.MapFrom(src => GetIdentificationReturnCode(src)))
                                                   .ForMember(d => d.Description, opt => opt.MapFrom(src => GetReturnCodeDesc(GetIdentificationReturnCode(src))))
                                                   .ForMember(d => d.Language, opt => opt.MapFrom(src => src.GetUserLanguageCode()))
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


