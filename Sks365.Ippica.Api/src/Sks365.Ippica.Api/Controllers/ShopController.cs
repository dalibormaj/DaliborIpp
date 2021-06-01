using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Api.Dto.Responses;
using Sks365.Ippica.Api.Utility;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.Ippica.Domain.Utility;
using Sks365.SessionTracker.Client;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sks365.Ippica.Api.Controllers
{
    [Route("v{version:apiVersion}/rest")]
    public class ShopController : BaseController
    {
        private readonly ILogger<ShopController> _logger;
        private readonly ISessionTracker _sessionTracker;
        private IMapper _mapper => MapperLocator.GetMapper(MapperName.ShopMapper);

        public ShopController(ILogger<ShopController> logger, ISessionTracker sessionTracker)
        {
            _logger = logger;
            _sessionTracker = sessionTracker;
        }

        [ApiVersion("1.0")]
        [HttpGet("Identification/{shopId}/{terminalId}/{operatorId}/{session}", Name = "Identification")]
        public async Task<ActionResult<ShopIdentificationResponse>> ShopIdentification(int shopId, int terminalId, int operatorId, string session)
        {
            var request = new ShopIdentificationRequest()
            {
                ShopId = shopId,
                TerminalId = terminalId,
                OperatorId = operatorId,
                Session = session,
            };
            GlobalValidator.Validate(request);

            var sessionData = await _sessionTracker.GetSession(session);
            User user = null;
            TerminalTypeEnum? terminalTypeId = null;
            if (sessionData.SessionExists && sessionData.ApplicationTypeId.HasValue)
            {
                user = UserService.GetUser(sessionData.Username, (BookmakerEnum)sessionData.BookmakerId);
                terminalTypeId = ((ApplicationTypeEnum)sessionData.ApplicationTypeId).ConvertToTerminalTypeEnum();
                user.AdditionalData = new List<UserAdditionalData>() { UserService.GetUserAdditionalDataValue((int)user.UserId, UserDataTypeEnum.Language) };
                user.AdditionalData = new List<UserAdditionalData>() { UserService.GetUserAdditionalDataValue((int)user.UserId, UserDataTypeEnum.ExalogicCodDiritto) };
            }

            user.ThrowIf(x => x == null || !terminalTypeId.HasValue, new SessionNotValidException());

            var response = _mapper.Map<ShopIdentificationResponse>(user);
            if (terminalTypeId.HasValue)
                response.Type = terminalTypeId.Value;

            //_logger.LogInformation("WebIdentification - Response -> response: {@response}", response);

            return Ok(response);
        }

        [ApiVersion("1.0")]
        [HttpGet("Identification/{terminalStr}/{session}/{ip}", Name = "TerminalIdentification")]
        public async Task<ActionResult<ShopIdentificationResponse>> ShopIdentificationByStr(string terminalStr, string session, string ip)
        {
            var request = new ShopIdentificationByStrRequest()
            {
                TerminalStr = terminalStr,
                Session = session,
                Ip = ip
            };
            GlobalValidator.Validate(request);

            var sessionData = await _sessionTracker.GetSession(session);
            User user = null;
            TerminalTypeEnum? terminalTypeId = null;
            if (sessionData.SessionExists && sessionData.ApplicationTypeId.HasValue)
            {
                user = UserService.GetUser(sessionData.Username, (BookmakerEnum)sessionData.BookmakerId);
                terminalTypeId = ((ApplicationTypeEnum)sessionData.ApplicationTypeId).ConvertToTerminalTypeEnum();
                user.AdditionalData = new List<UserAdditionalData>() { UserService.GetUserAdditionalDataValue((int)user.UserId, UserDataTypeEnum.Language) };
                user.AdditionalData = new List<UserAdditionalData>() { UserService.GetUserAdditionalDataValue((int)user.UserId, UserDataTypeEnum.ExalogicCodDiritto) };
            }

            user.ThrowIf(x => x == null || !terminalTypeId.HasValue, new SessionNotValidException());

            var response = _mapper.Map<ShopIdentificationResponse>(user);
            if (terminalTypeId.HasValue)
                response.Type = terminalTypeId.Value;

            //_logger.LogInformation("WebIdentification - Response -> response: {@response}", response);

            return Ok(response);
        }

        [ApiVersion("1.0")]
        [HttpGet("KeepAlive/{shopId}/{terminalId}/{operatorId}/{session}", Name = "KeepAlive")]
        public async Task<ActionResult<ShopKeepAliveResponse>> KeepAlive(int shopId, int terminalId, int operatorId, string session)
        {
            var request = new ShopKeepAliveRequest()
            {
                ShopId = shopId,
                TerminalId = terminalId,
                OperatorId = operatorId,
                Session = session
            };
            GlobalValidator.Validate(request);

            var sessionData = await _sessionTracker.GetSession(session);
            User user = null;
            TerminalTypeEnum? terminalTypeId = null;
            if (sessionData.SessionExists && sessionData.ApplicationTypeId.HasValue)
            {
                user = UserService.GetUser(sessionData.Username, (BookmakerEnum)sessionData.BookmakerId);
                terminalTypeId = ((ApplicationTypeEnum)sessionData.ApplicationTypeId).ConvertToTerminalTypeEnum();
            }

            user.ThrowIf(x => x == null || !terminalTypeId.HasValue, new SessionNotValidException());
            var response = _mapper.Map<ShopKeepAliveResponse>(user);

            return Ok(response);
        }

        [ApiVersion("1.0")]
        [HttpPost("ReserveBet", Name = "ReserveBet")]
        public async Task<ActionResult<ShopReserveBetResponse>> ReserveBet()
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var jsonRequest = await reader.ReadToEndAsync();

            var request = MstDeserializer.DeserializeShopReserve(jsonRequest);
            if (request == null) throw new IppicaException(ReturnCodeEnum.BadRequest, "Deserialization failed. Please check the json you are sending");

            GlobalValidator.Validate(request);

            var betRequest = _mapper.Map<BetRequest>(request);
            Bet bet = _mapper.Map<Bet>(request);
            var betTransactions = _mapper.Map<List<BetTransaction>>(request);

            bet = await BetService.Reserve(betRequest, bet, betTransactions);

            bet.ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.Unknown, "Something went wrong"));
            var response = _mapper.Map<ShopReserveBetResponse>(bet);

            return Ok(response);
        }

        [ApiVersion("1.0")]
        [HttpPost("PlaceBet", Name = "PlaceBet")]
        public async Task<ActionResult<ShopPlaceBetResponse>> PlaceBet()
        {
            using var reader = new StreamReader(Request.Body);
            var jsonRequest = await reader.ReadToEndAsync();

            var request = MstDeserializer.DeserializeShopPlace(jsonRequest);
            if (request == null) throw new IppicaException(ReturnCodeEnum.BadRequest, "Deserialization failed. Please check the json you are sending");

            GlobalValidator.Validate(request);

            var betRequest = _mapper.Map<BetRequest>(request);
            Bet bet = _mapper.Map<Bet>(request);
            var betTransactions = _mapper.Map<List<BetTransaction>>(request);

            bet = await BetService.Place(betRequest, bet, betTransactions);

            bet.ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.Unknown, "Something went wrong"));
            var response = _mapper.Map<ShopPlaceBetResponse>(bet);

            return Ok(response);
        }

        [ApiVersion("1.0")]
        [HttpPost("RollbackBet", Name = "RollbackBet")]
        public async Task<ActionResult<ShopRollbackBetResponse>> RollbackBet(ShopRollbackBetRequest request)
        {
            GlobalValidator.Validate(request);

            var betRequest = _mapper.Map<BetRequest>(request);

            var bet = await BetService.CancelStake(betRequest);

            bet.ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.Unknown, "Something went wrong"));
            var response = _mapper.Map<ShopRollbackBetResponse>(bet);

            return Ok(response);
        }

        [ApiVersion("1.0")]
        [HttpPost("CancelBet", Name = "CancelBet")]
        public async Task<ActionResult<ShopCancelBetResponse>> CancelBet(ShopCancelBetRequest request)
        {
            GlobalValidator.Validate(request);

            var betRequest = _mapper.Map<BetRequest>(request);
            var betTransactions = _mapper.Map<List<BetTransaction>>(request);

            var bet = await BetService.CancelStake(betRequest, betTransactions);

            bet.ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.Unknown, "Something went wrong"));
            var response = _mapper.Map<ShopCancelBetResponse>(bet);

            return Ok(response);
        }

        [ApiVersion("1.0")]
        [HttpPost("PayBet", Name = "PayBet")]
        public async Task<ActionResult<ShopPayBetResponse>> PayBet(ShopPayBetRequest request)
        {
            GlobalValidator.Validate(request);

            var betRequest = _mapper.Map<BetRequest>(request);
            var betTransactions = _mapper.Map<List<BetTransaction>>(request);

            Bet bet = null;
            switch (betRequest.ShopBetRequest.BetSettlementStatusId)
            {
                case BetSettlementStatusEnum.W:
                    bet = await BetService.SettleWin(betRequest, betTransactions);
                    break;
                case BetSettlementStatusEnum.L:
                    bet = await BetService.SettleLoss(betRequest);
                    break;
                case BetSettlementStatusEnum.V:
                    bet = await BetService.CancelStake(betRequest, betTransactions);
                    break;
            }

            bet.ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.Unknown, "Something went wrong"));
            var response = _mapper.Map<ShopPayBetResponse>(bet);

            return Ok(response);
        }

        [ApiVersion("1.0")]
        [HttpPost("SettleBet", Name = "SettleBet")]
        public async Task<ActionResult<ShopSettleBetResponse>> SettleBet(ShopSettleBetRequest request)
        {
            GlobalValidator.Validate(request);

            Bet bet = null;
            foreach (var ticket in request.Tickets)
            {
                var betRequest = _mapper.Map<BetRequest>(ticket);
                var betTransactions = _mapper.Map<List<BetTransaction>>(ticket);
                switch (betRequest.ShopBetRequest.BetSettlementStatusId)
                {
                    case BetSettlementStatusEnum.W:
                        bet = await BetService.SettleWin(betRequest, betTransactions, processTransactionsAsPending: true);
                        break;
                    case BetSettlementStatusEnum.L:
                        bet = await BetService.SettleLoss(betRequest);
                        break;
                    case BetSettlementStatusEnum.V:
                        bet = await BetService.CancelStake(betRequest, betTransactions, processTransactionsAsPending: true);
                        break;
                }

                bet.ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.Unknown, "Something went wrong"));
            }

            var response = _mapper.Map<ShopSettleBetResponse>(bet);
            return Ok(response);
        }
    }
}