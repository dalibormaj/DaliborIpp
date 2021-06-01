using Sks365.Ippica.Api.Dto;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System;

namespace Sks365.Ippica.Api.Utility
{
    public class BetRequestFactory
    {
        #region Web
        public static BetRequest Create(WebReserveBetRequest<BetDto> dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.WebReserveBet,
                WebBetRequest = new WebBetRequest() { GroupCode = dto.GroupCode, ShopCode = dto.ShopCode, Skin = dto.Skin },
                Session = dto.Session,
                TicketId = dto.TicketId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }

        public static BetRequest Create(WebReserveBetRequest<PsipBetDto> dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.WebReserveBet,
                WebBetRequest = new WebBetRequest() { GroupCode = dto.GroupCode, ShopCode = dto.ShopCode, Skin = dto.Skin },
                Session = dto.Session,
                TicketId = dto.TicketId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }

        public static BetRequest Create(WebReserveBetRequest<PsrBetDto> dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.WebReserveBet,
                WebBetRequest = new WebBetRequest() { GroupCode = dto.GroupCode, ShopCode = dto.ShopCode, Skin = dto.Skin },
                Session = dto.Session,
                TicketId = dto.TicketId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }


        public static BetRequest Create(WebPlaceBetRequest<BetDto> dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.WebPlaceBet,
                WebBetRequest = new WebBetRequest() { GroupCode = dto.GroupCode, ShopCode = dto.ShopCode, Skin = dto.Skin },
                Session = dto.Session,
                TicketId = dto.TicketId,
                ExternalId = dto.ExternalId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }

        public static BetRequest Create(WebPlaceBetRequest<PsipBetDto> dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.WebPlaceBet,
                WebBetRequest = new WebBetRequest() { GroupCode = dto.GroupCode, ShopCode = dto.ShopCode, Skin = dto.Skin },
                Session = dto.Session,
                TicketId = dto.TicketId,
                ExternalId = dto.ExternalId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }

        public static BetRequest Create(WebPlaceBetRequest<PsrBetDto> dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.WebPlaceBet,
                WebBetRequest = new WebBetRequest() { GroupCode = dto.GroupCode, ShopCode = dto.ShopCode, Skin = dto.Skin },
                Session = dto.Session,
                TicketId = dto.TicketId,
                ExternalId = dto.ExternalId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }

        public static BetRequest Create(WebRollbackBetRequest dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.WebRollbackBet,
                WebBetRequest = new WebBetRequest() { GroupCode = dto.GroupCode, ShopCode = dto.ShopCode, Skin = dto.Skin },
                Session = dto.Session,
                TicketId = dto.TicketId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }

        public static BetRequest Create(WebSettleBetRequest dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.WebSettleBet,
                WebBetRequest = new WebBetRequest()
                {
                    GroupCode = dto.GroupCode,
                    ShopCode = dto.ShopCode,
                    Skin = dto.Skin,
                    UserAccount = dto.UserAccount,
                    BetSettlementId = dto.Id,
                    BetSettlementReasonId = dto.Reason
                },
                ExternalId = dto.TicketId, //Special condition just for Settlement. MST made this confusion
                Game = dto.Game,
                Timestamp = dto.Timestamp,
            };

            return result;
        }

        #endregion

        #region Shop
        public static BetRequest Create(ShopReserveBetRequest<BetDto> dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.ShopReserveBet,
                ShopBetRequest = new ShopBetRequest() { ShopId = dto.ShopId, TerminalId = dto.TerminalId, TerminalStr = dto.TerminalStr },
                Session = dto.Session,
                TicketId = dto.TicketId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }

        public static BetRequest Create(ShopReserveBetRequest<PsipBetDto> dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.ShopReserveBet,
                ShopBetRequest = new ShopBetRequest() { ShopId = dto.ShopId, TerminalId = dto.TerminalId, TerminalStr = dto.TerminalStr },
                Session = dto.Session,
                TicketId = dto.TicketId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }

        public static BetRequest Create(ShopReserveBetRequest<PsrBetDto> dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.ShopReserveBet,
                ShopBetRequest = new ShopBetRequest() { ShopId = dto.ShopId, TerminalId = dto.TerminalId, TerminalStr = dto.TerminalStr },
                Session = dto.Session,
                TicketId = dto.TicketId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }


        public static BetRequest Create(ShopPlaceBetRequest<BetDto> dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.ShopPlaceBet,
                ShopBetRequest = new ShopBetRequest() { ShopId = dto.ShopId, TerminalId = dto.TerminalId, TerminalStr = dto.TerminalStr, Loyalty = dto.Loyalty },
                Session = dto.Session,
                TicketId = dto.TicketId,
                ExternalId = dto.ExternalId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp
            };

            return result;
        }

        public static BetRequest Create(ShopPlaceBetRequest<PsipBetDto> dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.ShopPlaceBet,
                ShopBetRequest = new ShopBetRequest() { ShopId = dto.ShopId, TerminalId = dto.TerminalId, TerminalStr = dto.TerminalStr, Loyalty = dto.Loyalty },
                Session = dto.Session,
                TicketId = dto.TicketId,
                ExternalId = dto.ExternalId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp
            };

            return result;
        }

        public static BetRequest Create(ShopPlaceBetRequest<PsrBetDto> dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.ShopPlaceBet,
                ShopBetRequest = new ShopBetRequest() { ShopId = dto.ShopId, TerminalId = dto.TerminalId, TerminalStr = dto.TerminalStr, Loyalty = dto.Loyalty },
                Session = dto.Session,
                TicketId = dto.TicketId,
                ExternalId = dto.ExternalId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }

        public static BetRequest Create(ShopRollbackBetRequest dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.ShopRollbackBet,
                ShopBetRequest = new ShopBetRequest() { ShopId = dto.ShopId, TerminalId = dto.TerminalId, TerminalStr = dto.TerminalStr },
                Session = dto.Session,
                TicketId = dto.TicketId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }

        public static BetRequest Create(ShopCancelBetRequest dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.ShopCancelBet,
                ShopBetRequest = new ShopBetRequest() { ShopId = dto.ShopId, TerminalId = dto.TerminalId, TerminalStr = dto.TerminalStr },
                Session = dto.Session,
                ExternalId = dto.TicketId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            return result;
        }

        public static BetRequest Create(TicketDto dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.ShopSettleBet,
                ShopBetRequest = new ShopBetRequest() { ShopId = dto.ShopId, BetSettlementStatusId = (BetSettlementStatusEnum)Enum.Parse(typeof(BetSettlementStatusEnum), dto.Status) },
                ExternalId = dto.TicketId, //Special condition just for Settlement. MST made this confusion
                Game = dto.Game
            };

            return result;
        }

        public static BetRequest Create(ShopPayBetRequest dto)
        {
            var result = new BetRequest
            {
                BetRequestTypeId = BetRequestTypeEnum.PayBet,
                ShopBetRequest = new ShopBetRequest() { ShopId = dto.ShopId, TerminalId = dto.TerminalId, TerminalStr = dto.TerminalStr },
                Session = dto.Session,
                ExternalId = dto.TicketId,
                Game = dto.Game,
                Ip = dto.Ip,
                Timestamp = dto.Timestamp,
            };

            result.ShopBetRequest.BetSettlementStatusId = (dto.WinningAmount > 0) ? BetSettlementStatusEnum.W :
                                                          (dto.RefundableAmount > 0) ? BetSettlementStatusEnum.V : BetSettlementStatusEnum.L;

            return result;
        }

        #endregion

    }
}
