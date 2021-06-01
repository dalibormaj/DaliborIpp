using AutoMapper;
using Microsoft.AspNetCore.Http;
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
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Sks365.Ippica.Api.Controllers
{
    [Route("v{version:apiVersion}/rest")]
    public class WebController : BaseController
    {
        private readonly ILogger<WebController> _logger;
        private IMapper _mapper => MapperLocator.GetMapper(MapperName.WebMapper);

        public WebController(ILogger<WebController> logger)
        {
            _logger = logger;
        }

        [ApiVersion("1.0")]
        [HttpGet("WebIdentification/{groupCode}/{shopCode}/{session}/{ip}", Name = "WebIdentification")]
        public ActionResult<WebIdentificationResponse> WebIdentification(string groupCode, string shopCode, string session, string ip)
        {
            // _logger.LogInformation("WebIdentification - Request -> groupCode: {groupCode} shopCode: {shopCode} session: {session}, ip: {ip}", groupCode, shopCode, session, ip);

            var request = new WebIdentificationRequest()
            {
                GroupCode = groupCode,
                ShopCode = shopCode,
                Session = session,
                Ip = ip
            };
            GlobalValidator.Validate(request);

            var user = UserService.GetUser(session);
            user.ThrowIf(x => x == null, new SessionNotValidException());
            user.AdditionalData = new List<UserAdditionalData>() { UserService.GetUserAdditionalDataValue((int)user.UserId, UserDataTypeEnum.Language) };

            var response = _mapper.Map<WebIdentificationResponse>(user);
            response.Session = session;

            //_logger.LogInformation("WebIdentification - Response -> response: {@response}", response);

            return Ok(response);
        }


        [ApiVersion("1.0")]
        [HttpGet("WebUserBalance/{groupCode}/{shopCode}/{session}/{ip}", Name = "WebUserBalance")]
        public ActionResult<WebUserBalanceResponse> WebUserBalance(string groupCode, string shopCode, string session, string ip)
        {
            //_logger.LogInformation("WebUserBalance - Request -> groupCode: {groupCode} shopCode: {shopCode} session: {session}, ip: {ip}", groupCode, shopCode, session, ip);

            var request = new WebUserBalanceRequest()
            {
                Session = session,
                Ip = ip
            };

            GlobalValidator.Validate(request);

            var user = UserService.GetUser(session, getWallets: true);
            user.ThrowIf(x => x == null, new SessionNotValidException());
            user.AdditionalData = new List<UserAdditionalData>() { UserService.GetUserAdditionalDataValue((int)user.UserId, UserDataTypeEnum.Language) };

            var response = _mapper.Map<WebUserBalanceResponse>(user);

            //_logger.LogInformation("WebUserBalance - Response -> response: {@response}", response);

            return Ok(response);
        }

        [ApiVersion("1.0")]
        [HttpGet("WebKeepAlive/{groupCode}/{shopCode}/{session}/{ip}", Name = "WebKeepAlive")]
        public ActionResult<WebKeepAliveResponse> WebKeepAlive(string groupCode, string shopCode, string session, string ip)
        {
            //_logger.LogInformation("WebKeepAlive - Request -> groupCode: {groupCode} shopCode: {shopCode} session: {session}, ip: {ip}", groupCode, shopCode, session, ip);

            var request = new WebKeepAliveRequest()
            {
                GroupCode = groupCode,
                ShopCode = shopCode,
                Session = session,
                Ip = ip
            };

            GlobalValidator.Validate(request);

            var user = UserService.GetUser(session);
            user.ThrowIf(x => x == null, new SessionNotValidException());
            user.AdditionalData = new List<UserAdditionalData>() { UserService.GetUserAdditionalDataValue((int)user.UserId, UserDataTypeEnum.Language) };

            var response = _mapper.Map<WebKeepAliveResponse>(user);

            // _logger.LogInformation("WebKeepAlive - Response -> response: {@response}", response);

            return response;
        }

        [ApiVersion("1.0")]
        [HttpPost("WebReserveBet", Name = "WebReserveBet")]
        public async Task<ActionResult<WebReserveBetResponse>> ReserveBet()
        {
            var reader = new StreamReader(HttpContext.Request.Body);
            var jsonRequest = await reader.ReadToEndAsync();

            var request = MstDeserializer.DeserializeWebReserve(jsonRequest);
            if (request == null) throw new IppicaException(ReturnCodeEnum.BadRequest, "Deserialization failed. Please check the json you are sending");

            GlobalValidator.Validate(request);

            //_logger.LogInformation("ReserveBet - Request -> {request}", jsonRequest);

            var betRequest = _mapper.Map<BetRequest>(request);
            Bet bet = _mapper.Map<Bet>(request);
            var betTransactions = _mapper.Map<List<BetTransaction>>(request);

            bet = await BetService.Reserve(betRequest, bet, betTransactions);

            bet.ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.Unknown, "Something went wrong"));
            var response = _mapper.Map<WebReserveBetResponse>(bet);

            //_logger.LogInformation("ReserveBet - Response -> {@response}", response);

            return Ok(response);
        }

        [ApiVersion("1.0")]
        [HttpPost("WebPlaceBet", Name = "WebPlaceBet")]
        public async Task<ActionResult<WebPlaceBetResponse>> PlaceBet()
        {
            using var reader = new StreamReader(Request.Body);
            var jsonRequest = await reader.ReadToEndAsync();

            var request = MstDeserializer.DeserializeWebPlace(jsonRequest);
            if (request == null) throw new IppicaException(ReturnCodeEnum.BadRequest, "Deserialization failed. Please check the json you are sending");

            GlobalValidator.Validate(request);

            //_logger.LogInformation("PlaceBet - Request -> {request}", jsonRequest);

            var betRequest = _mapper.Map<BetRequest>(request);
            Bet bet = _mapper.Map<Bet>(request);
            var betTransactions = _mapper.Map<List<BetTransaction>>(request);

            bet = await BetService.Place(betRequest, bet, betTransactions);

            bet.ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.Unknown, "Something went wrong"));
            var response = _mapper.Map<WebPlaceBetResponse>(bet);

            //_logger.LogInformation("PlaceBet - Response -> {@response}", response);

            return Ok(response);
        }

        [ApiVersion("1.0")]
        [HttpPost("WebRollbackBet", Name = "WebRollbackBet")]
        public async Task<ActionResult<WebRollbackBetResponse>> RollbackBet(WebRollbackBetRequest request)
        {
            GlobalValidator.Validate(request);

            //_logger.LogInformation("RollbackBet - Request -> {@request}", (object)request);

            var betRequest = _mapper.Map<BetRequest>(request);

            var bet = await BetService.CancelStake(betRequest);

            bet.ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.Unknown, "Something went wrong"));
            var response = _mapper.Map<WebRollbackBetResponse>(bet);

            //_logger.LogInformation("RollbackBet - Response -> {@response}", response);

            return Ok(response);
        }


        [ApiVersion("1.0")]
        [HttpPost("WebSettleBet", Name = "WebSettleBet")]
        public async Task<ActionResult<WebSettleBetResponse>> SettleBet(WebSettleBetRequest request)
        {
            GlobalValidator.Validate(request);

            //_logger.LogInformation("SettleBet - Request -> {@request}", (object)request);

            var betRequest = _mapper.Map<BetRequest>(request);
            var betTransactions = _mapper.Map<List<BetTransaction>>(request);
            Bet bet = null;

            switch (betRequest.WebBetRequest.BetSettlementReasonId)
            {
                case BetSettlementReasonEnum.Payment:
                    bet = await BetService.SettleWin(betRequest, betTransactions);
                    break;
                case BetSettlementReasonEnum.CancelPayment:
                    bet = await BetService.CancelWin(betRequest, betTransactions);
                    break;
                case BetSettlementReasonEnum.Losers:
                    bet = await BetService.SettleLoss(betRequest);
                    break;
                case BetSettlementReasonEnum.TicketReopened:
                    bet = await BetService.Reopen(betRequest);
                    break;
                case BetSettlementReasonEnum.PaymentAndRefund:
                    bet = await BetService.SettleWin(betRequest, betTransactions);
                    break;
                case BetSettlementReasonEnum.CancelPaymentAndRefund:
                    bet = await BetService.CancelWin(betRequest, betTransactions);
                    break;
                case BetSettlementReasonEnum.Refund:
                    bet = await BetService.CancelStake(betRequest, betTransactions);
                    break;
                case BetSettlementReasonEnum.CancelRefund:
                    bet = await BetService.UndoCancelStake(betRequest, betTransactions);
                    break;
            }
            bet.ThrowIf(x => x == null, new IppicaException(ReturnCodeEnum.Unknown, "Something went wrong"));
            var response = _mapper.Map<WebSettleBetResponse>(bet);

            //_logger.LogInformation("SettleBet - Response -> {@response}", response);

            return Ok(response);
        }
    }
}