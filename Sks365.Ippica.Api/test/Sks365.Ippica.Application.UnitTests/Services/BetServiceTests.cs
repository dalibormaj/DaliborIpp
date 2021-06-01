using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Sks365.Ippica.Application.Services;
using Sks365.Ippica.Application.Services.Abstraction;
using Sks365.Ippica.Application.UnitTests.Helpers;
using Sks365.Ippica.Application.Utility.EmailSender;
using Sks365.Ippica.Application.Utility.OperationRecorder;
using Sks365.Ippica.Common.Config.Abstraction;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.DataAccess;
using Sks365.Ippica.DataAccess.Repositories.Abstraction;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.Ippica.Domain.Utility;
using Sks365.Ippica.FakeData.Domain;
using Sks365.SessionTracker.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sks365.Ippica.Application.UnitTests.Services
{
    public class BetServiceTests
    {
        private Mock<IHttpContextAccessor> _httpContextAccessor;
        private Mock<HttpContext> _httpContext;
        private Mock<IServiceProvider> _serviceProvider;
        private Mock<IMstUnitOfWork> _mstUnitOfWork;
        private Mock<IIsbetsUnitOfWork> _isbetsUnitOfWork;
        private Mock<IBetRepository> _betRepository;
        private Mock<IPaymentOrderService> _paymentOrderService;
        private Mock<IUserService> _userService;
        private Mock<IBetService> _betService;
        private Mock<IEmailSender> _emailSender;
        private Mock<ISessionTracker> _sessionTracker;
        private Mock<ILogger<BetService>> _logger;
        private Mock<IOperationRecorder> _operationRecorder;
        private Mock<IAppSettings> _appSettings;
        private Mock<IOperationRecorderExecutor> _operationRecorderExecutor;

        public BetServiceTests()
        {
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _httpContext = new Mock<HttpContext>();
            _serviceProvider = new Mock<IServiceProvider>();
            _mstUnitOfWork = new Mock<IMstUnitOfWork>();
            _isbetsUnitOfWork = new Mock<IIsbetsUnitOfWork>();
            _betRepository = new Mock<IBetRepository>();
            _paymentOrderService = new Mock<IPaymentOrderService>();
            _userService = new Mock<IUserService>();
            _betService = new Mock<IBetService>();
            _emailSender = new Mock<IEmailSender>();
            _sessionTracker = new Mock<ISessionTracker>();
            _logger = new Mock<ILogger<BetService>>();
            _operationRecorder = new Mock<IOperationRecorder>();
            _operationRecorderExecutor = new Mock<IOperationRecorderExecutor>();
            _appSettings = new Mock<IAppSettings>();

            //default
            MockUserService(null);
            MockEmailSender();
            MockOperationRecorder();

            _betRepository.Setup(x => x.SaveBet(It.IsAny<Bet>())).Returns<Bet>(x => { x.BetId = 11111; return x; });
            _betRepository.Setup(x => x.SaveBetRequest(It.IsAny<BetRequest>())).Returns<BetRequest>(x => { x.BetRequestId = 222222; return x; });
            _betRepository.Setup(x => x.SaveBetTransaction(It.IsAny<BetTransaction>())).Returns<BetTransaction>(x => { x.BetTransactionId = 333333; return x; });

            _paymentOrderService.Setup(x => x.ProcessTransaction(It.IsAny<BetTransaction>(), It.IsAny<int>(), It.IsAny<BookmakerEnum>(), It.IsAny<string>(), It.IsAny<bool>())).Returns<BetTransaction, int, BookmakerEnum, string, bool>((betTran, userId, bookmakerId, ip, proccAsPending) =>
            {
                betTran.PaymentOrderId = new Random().Next(20000000, 40000000);
                betTran.BetTransactionId = new Random().Next(20000000, 40000000);
                return Task.FromResult(betTran);
            });

            Link();
        }

        private void MockUserService(int? userId)
        {
            var fakeUser = new FakeUser(userId).FakeData.Generate();
            var fakeSportWallet = new FakeSportWallet().FakeData.Generate();
            _userService.Setup(x => x.GetUser(It.IsAny<string>(), It.IsAny<BookmakerEnum>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fakeUser);
            _userService.Setup(x => x.GetUser(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fakeUser);
            _userService.Setup(x => x.GetUser(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fakeUser);
            _userService.Setup(x => x.GetUserByTicketId(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(fakeUser);
            _userService.Setup(x => x.GetSportWallet(It.IsAny<int>())).Returns(fakeSportWallet);
        }

        private void MockEmailSender()
        {
            _emailSender.Setup(x => x.SendEmail(It.IsAny<Email>(), It.IsAny<EmailRepetitionEnum>()));
        }

        private void MockOperationRecorder()
        {
            _operationRecorder.Setup(x => x.CreateExecutor(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BetRequestTypeEnum>())).Returns(_operationRecorderExecutor.Object);
        }

        private void Link()
        {
            _mstUnitOfWork.SetupGet(x => x.BetRepository).Returns(_betRepository.Object);

            _serviceProvider.Setup(x => x.GetService(typeof(IUserService))).Returns(_userService.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IBetService))).Returns(_betService.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IPaymentOrderService))).Returns(_paymentOrderService.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IMstUnitOfWork))).Returns(_mstUnitOfWork.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IIsbetsUnitOfWork))).Returns(_isbetsUnitOfWork.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IEmailSender))).Returns(_emailSender.Object);

            _httpContext.SetupGet(x => x.RequestServices).Returns(_serviceProvider.Object);
            _httpContextAccessor.SetupGet(x => x.HttpContext).Returns(_httpContext.Object);
        }

        [Theory]
        [InlineData(BetHelperAction.None, BetTransactionTypeEnum.Stake)]
        [InlineData(BetHelperAction.Create_Fix_Reserved, BetTransactionTypeEnum.Stake)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.Stake)]
        [InlineData(BetHelperAction.Create_Fix_Won, BetTransactionTypeEnum.Stake, BetTransactionTypeEnum.RefundTaxWin)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.Stake)]
        public async Task ReserveBet(BetHelperAction action, params BetTransactionTypeEnum[] newTransactionTypes)
        {
            //Arrange
            var typeId = BetHelper.ParseTypeId(action);

            //current state
            var fakeHelper = new BetHelper(action);
            var fakeBet = fakeHelper.GetBet();
            var fakeLastTransactions = fakeHelper.GetLastBetTransactions();
            var fakeStake = fakeHelper.GetBetTransaction(BetTransactionTypeEnum.Stake);

            //new data
            var fakeNewBet = (fakeBet == null) ? BetHelper.Clear(BetHelper.CreateBet(typeId)) :
                                                 BetHelper.Clear(fakeBet);
            var fakeNewBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeNewBet, BetRequestTypeEnum.WebReserveBet));
            var fakeNewTransactions = BetHelper.Clear(BetHelper.CreateBetTransactions(fakeNewBet, newTransactionTypes));

            //mock
            _betRepository.Setup(x => x.GetBet(It.IsAny<long>(), It.IsAny<bool>())).Returns(fakeBet);
            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet);
            _betRepository.Setup(x => x.GetBetByExternalId(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet);
            _betRepository.Setup(x => x.GetLastBetTransactions(It.IsAny<long>(), false)).Returns(fakeLastTransactions);
            MockUserService(fakeNewBet?.UserId);

            var betService = new BetService(_httpContextAccessor.Object,
                                            _paymentOrderService.Object,
                                            _userService.Object,
                                            _emailSender.Object,
                                            _operationRecorder.Object,
                                            _sessionTracker.Object,
                                            _logger.Object,
                                            _appSettings.Object);

            //Act
            Task<Bet> reserve() => betService.Reserve(fakeNewBetRequest, fakeNewBet, fakeNewTransactions);

            //Assert
            var supportedStatuses = new List<BetStatusEnum>() { BetStatusEnum.Reserved };
            var supportedTransactions = new List<BetTransactionTypeEnum>() { BetTransactionTypeEnum.Stake,
                                                                             BetTransactionTypeEnum.TaxStake };
            var isDataMismatched = fakeBet?.BetStatusId == BetStatusEnum.Reserved && //retry
                                   !fakeLastTransactions.CompareList(fakeNewTransactions);
            var isStatusSupported = supportedStatuses.Exists(x => fakeBet == null || x == fakeBet?.BetStatusId);
            var containsUnsupportedTransaction = fakeNewTransactions.Exists(x => !supportedTransactions.Exists(y => y == x.BetTransactionTypeId));

            if (isDataMismatched || !isStatusSupported || containsUnsupportedTransaction)
            {
                var exception = await Assert.ThrowsAsync<IppicaException>(reserve);
            }
            else
            {
                fakeBet = await reserve();
                Assert.NotNull(fakeBet);
                Assert.Equal(BetStatusEnum.Reserved, fakeBet.BetStatusId);
                if (fakeStake != null)
                    Assert.Equal(Math.Abs(fakeStake.Amount ?? 0), Math.Abs(fakeBet.Stake ?? 0));
                Assert.Equal(0, fakeBet.WinAmount ?? 0);
                Assert.Equal(0, fakeBet.RefundAmount ?? 0);
            }
        }


        [Theory]
        [InlineData(BetHelperAction.Create_Fix_Reserved, BetTransactionTypeEnum.Stake)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.Stake)]
        [InlineData(BetHelperAction.Create_Fix_Won, BetTransactionTypeEnum.Stake, BetTransactionTypeEnum.RefundTaxWin)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.Stake)]
        [InlineData(BetHelperAction.Create_Psip_UndoRefund, BetTransactionTypeEnum.Stake)]
        public async Task PlaceBet(BetHelperAction action, params BetTransactionTypeEnum[] newTransactionTypes)
        {
            //Arrange

            //current state
            var fakeHelper = new BetHelper(action);
            var fakeBet = fakeHelper.GetBet();
            var fakeLastTransactions = fakeHelper.GetLastBetTransactions();
            var fakeStake = fakeHelper.GetBetTransaction(BetTransactionTypeEnum.Stake);

            //new data
            var fakeNewBet = BetHelper.Clear(fakeBet);
            var fakeNewBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeNewBet, BetRequestTypeEnum.WebPlaceBet));
            var fakeNewTransactions = BetHelper.Clear(BetHelper.CreateBetTransactions(fakeNewBet, newTransactionTypes));

            //mock
            _betRepository.Setup(x => x.GetBet(It.IsAny<long>(), It.IsAny<bool>())).Returns(fakeBet);
            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet);
            _betRepository.Setup(x => x.GetBetByExternalId(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetLastBetTransactions(It.IsAny<long>(), false)).Returns(fakeLastTransactions);
            MockUserService(fakeBet?.UserId);

            var betService = new BetService(_httpContextAccessor.Object,
                                            _paymentOrderService.Object,
                                            _userService.Object,
                                            _emailSender.Object,
                                            _operationRecorder.Object,
                                            _sessionTracker.Object,
                                            _logger.Object,
                                            _appSettings.Object);

            //Act
            Task<Bet> place() => betService.Place(fakeNewBetRequest, fakeNewBet, fakeNewTransactions);

            //Assert
            var supportedStatuses = new List<BetStatusEnum>() { BetStatusEnum.Reserved, BetStatusEnum.Placed };
            var supportedTransactions = new List<BetTransactionTypeEnum>() { BetTransactionTypeEnum.Stake,
                                                                             BetTransactionTypeEnum.TaxStake };
            var isDataMismatched = fakeBet.BetStatusId == BetStatusEnum.Reserved && //retry
                                   !fakeLastTransactions.CompareList(fakeNewTransactions);
            var isStatusSupported = supportedStatuses.Exists(x => x == fakeBet.BetStatusId);
            var containsUnsupportedTransaction = fakeNewTransactions.Exists(x => !supportedTransactions.Exists(y => y == x.BetTransactionTypeId));

            if (isDataMismatched || !isStatusSupported || containsUnsupportedTransaction)
            {
                var exception = await Assert.ThrowsAsync<IppicaException>(place);
            }
            else
            {
                fakeBet = await place();
                Assert.NotNull(fakeBet);
                Assert.Equal(BetStatusEnum.Placed, fakeBet.BetStatusId);
                Assert.Equal(Math.Abs(fakeStake.Amount ?? 0), Math.Abs(fakeBet.Stake ?? 0));
                Assert.Equal(0, fakeBet.WinAmount);
                Assert.Equal(0, fakeBet.RefundAmount);
            }
        }


        [Theory]
        [InlineData(BetHelperAction.Create_Fix_Reserved, BetTransactionTypeEnum.Stake)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psr_Placed, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psr_Placed, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psip_Placed, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Placed, BetTransactionTypeEnum.Win)]

        [InlineData(BetHelperAction.Create_Fix_WonNotPaid, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_WonNotPaid, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psr_WonNotPaid, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psr_WonNotPaid, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psip_WonNotPaid, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_WonNotPaid, BetTransactionTypeEnum.Win)]

        [InlineData(BetHelperAction.Create_Fix_Won, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_Won, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psr_Won, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psr_Won, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psip_Won, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Won, BetTransactionTypeEnum.Win)]

        [InlineData(BetHelperAction.Create_Psr_Won_IncludingStakeCompensation, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psr_Won_IncludingStakeCompensation, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psip_Won_IncludingStakeCompensation, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Won_IncludingStakeCompensation, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psip_UndoRefund, BetTransactionTypeEnum.Win)]
        public async Task SettleWin(BetHelperAction action, params BetTransactionTypeEnum[] newTransactionTypes)
        {
            //Arrange

            //current state
            var fakeHelper = new BetHelper(action);
            var fakeBet = fakeHelper.GetBet();
            var fakeLastTransactions = fakeHelper.GetLastBetTransactions();
            var fakeStake = fakeHelper.GetBetTransaction(BetTransactionTypeEnum.Stake);

            //new data
            var fakeNewBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeBet, BetRequestTypeEnum.WebSettleBet));
            var fakeNewTransactions = BetHelper.Clear(BetHelper.CreateBetTransactions(fakeBet, newTransactionTypes));
            var fakeNewWin = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.Win);
            var fakeNewStakeCompensation = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.StakeCompensation);

            //mock
            _betRepository.Setup(x => x.GetBet(It.IsAny<long>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBetByExternalId(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, false)).Returns(fakeStake);
            _betRepository.Setup(x => x.GetLastBetTransactions(It.IsAny<long>(), false)).Returns(fakeLastTransactions);
            MockUserService(fakeBet?.UserId);

            var betService = new BetService(_httpContextAccessor.Object,
                                            _paymentOrderService.Object,
                                            _userService.Object,
                                            _emailSender.Object,
                                            _operationRecorder.Object,
                                            _sessionTracker.Object,
                                            _logger.Object,
                                            _appSettings.Object);

            //Act
            Task<Bet> settleWin() => betService.SettleWin(fakeNewBetRequest, fakeNewTransactions);

            //Assert
            var supportedStatuses = new List<BetStatusEnum>() { BetStatusEnum.Placed, BetStatusEnum.WonNotPaid, BetStatusEnum.Won };
            var supportedTransactions = new List<BetTransactionTypeEnum>() { BetTransactionTypeEnum.Win, BetTransactionTypeEnum.TaxWin,
                                                                             BetTransactionTypeEnum.TaxStake, BetTransactionTypeEnum.StakeCompensation };
            var isDataMismatched = (fakeBet.BetStatusId == BetStatusEnum.Won || fakeBet.BetStatusId == BetStatusEnum.WonNotPaid) && //retry
                                    !fakeLastTransactions.CompareList(fakeNewTransactions);
            var isStatusSupported = supportedStatuses.Exists(x => x == fakeBet.BetStatusId);
            var containsUnsupportedTransaction = fakeNewTransactions.Exists(x => !supportedTransactions.Exists(y => y == x.BetTransactionTypeId));
            var isPartialRefundForFixBet = fakeBet.BetTypeId == BetTypeEnum.Fix && fakeNewStakeCompensation != null;

            if (isDataMismatched || !isStatusSupported || isPartialRefundForFixBet || containsUnsupportedTransaction)
            {
                var exception = await Assert.ThrowsAsync<IppicaException>(settleWin);
            }
            else
            {
                fakeBet = await settleWin();
                Assert.NotNull(fakeBet);
                Assert.Equal(Math.Abs(fakeNewWin.Amount ?? 0), Math.Abs(fakeBet.WinAmount ?? 0));
                Assert.Equal(Math.Abs(fakeNewStakeCompensation?.Amount ?? 0), Math.Abs(fakeBet.RefundAmount ?? 0));
                Assert.Equal(BetStatusEnum.Won, fakeBet.BetStatusId);
            }
        }

        [Theory]
        [InlineData(BetHelperAction.Create_Fix_Reserved, BetTransactionTypeEnum.Stake)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psr_Placed, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psr_Placed, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psip_Placed, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Placed, BetTransactionTypeEnum.Win)]

        [InlineData(BetHelperAction.Create_Fix_WonNotPaid, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_WonNotPaid, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psr_WonNotPaid, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psr_WonNotPaid, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psip_WonNotPaid, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_WonNotPaid, BetTransactionTypeEnum.Win)]

        [InlineData(BetHelperAction.Create_Fix_Won, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_Won, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psr_Won, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psr_Won, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psip_Won, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Won, BetTransactionTypeEnum.Win)]

        [InlineData(BetHelperAction.Create_Psr_Won_IncludingStakeCompensation, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psr_Won_IncludingStakeCompensation, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psip_Won_IncludingStakeCompensation, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Won_IncludingStakeCompensation, BetTransactionTypeEnum.Win)]
        [InlineData(BetHelperAction.Create_Psip_UndoRefund, BetTransactionTypeEnum.Win)]
        public async Task SettleWinNotPaid(BetHelperAction action, params BetTransactionTypeEnum[] newTransactionTypes)
        {
            //Arrange

            //current state
            var fakeHelper = new BetHelper(action);
            var fakeBet = fakeHelper.GetBet();
            var fakeLastTransactions = fakeHelper.GetLastBetTransactions();
            var fakeStake = fakeHelper.GetBetTransaction(BetTransactionTypeEnum.Stake);

            //new data
            var fakeNewBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeBet, BetRequestTypeEnum.WebSettleBet));
            var fakeNewTransactions = BetHelper.Clear(BetHelper.CreateBetTransactions(fakeBet, newTransactionTypes));
            var fakeNewWin = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.Win);
            var fakeNewStakeCompensation = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.StakeCompensation);

            //mock
            _betRepository.Setup(x => x.GetBet(It.IsAny<long>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBetByExternalId(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, false)).Returns(fakeStake);
            _betRepository.Setup(x => x.GetLastBetTransactions(It.IsAny<long>(), false)).Returns(fakeLastTransactions);
            MockUserService(fakeBet?.UserId);

            var betService = new BetService(_httpContextAccessor.Object,
                                            _paymentOrderService.Object,
                                            _userService.Object,
                                            _emailSender.Object,
                                            _operationRecorder.Object,
                                            _sessionTracker.Object,
                                            _logger.Object,
                                            _appSettings.Object);

            //Act
            Task<Bet> settleWin() => betService.SettleWin(fakeNewBetRequest, fakeNewTransactions, true);

            //Assert
            var supportedStatuses = new List<BetStatusEnum>() { BetStatusEnum.Placed, BetStatusEnum.WonNotPaid };
            var supportedTransactions = new List<BetTransactionTypeEnum>() { BetTransactionTypeEnum.Win, BetTransactionTypeEnum.TaxWin,
                                                                             BetTransactionTypeEnum.TaxStake, BetTransactionTypeEnum.StakeCompensation };
            var isDataMismatched = fakeBet.BetStatusId == BetStatusEnum.WonNotPaid && !fakeLastTransactions.CompareList(fakeNewTransactions); //retry
            var isStatusSupported = supportedStatuses.Exists(x => x == fakeBet.BetStatusId);
            var containsUnsupportedTransaction = fakeNewTransactions.Exists(x => !supportedTransactions.Exists(y => y == x.BetTransactionTypeId));
            var isPartialRefundForFixBet = fakeBet.BetTypeId == BetTypeEnum.Fix && fakeNewStakeCompensation != null;

            if (isDataMismatched || !isStatusSupported || isPartialRefundForFixBet || containsUnsupportedTransaction)
            {
                var exception = await Assert.ThrowsAsync<IppicaException>(settleWin);
            }
            else
            {
                fakeBet = await settleWin();
                Assert.NotNull(fakeBet);
                Assert.Equal(Math.Abs(fakeNewWin.Amount ?? 0), Math.Abs(fakeBet.WinAmount ?? 0));
                Assert.Equal(Math.Abs(fakeNewStakeCompensation?.Amount ?? 0), Math.Abs(fakeBet.RefundAmount ?? 0));
                Assert.Equal(BetStatusEnum.WonNotPaid, fakeBet.BetStatusId);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task SettleWin_Retry(bool isValidRetry)
        {
            //Arrange

            //current state
            var fakeHelper = new BetHelper(BetHelperAction.Create_Psip_Won_IncludingStakeCompensation);
            var fakeBet = fakeHelper.GetBet();
            var fakeLastTransactions = fakeHelper.GetLastBetTransactions();
            var fakeStake = fakeHelper.GetBetTransaction(BetTransactionTypeEnum.Stake);

            //new data
            var fakeNewBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeBet, BetRequestTypeEnum.WebSettleBet));
            var fakeNewTransactions = BetHelper.Clear(BetHelper.CreateBetTransactions(fakeBet, BetTransactionTypeEnum.Win, BetTransactionTypeEnum.StakeCompensation));
            var fakeNewWin = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.Win);
            var fakeNewStakeCompensation = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.StakeCompensation);

            //In case of invalid retry fakeNewTransactions and fakeLastTransactions should differs
            if (!isValidRetry)
            {
                fakeNewTransactions.ForEach(x =>
                {
                    //set a random value
                    x.Amount = Math.Round((decimal)new Random().NextDouble() * new Random().Next(10, 30), 2);
                });
            }

            //mock
            _betRepository.Setup(x => x.GetBet(It.IsAny<long>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBetByExternalId(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, false)).Returns(fakeStake);
            _betRepository.Setup(x => x.GetLastBetTransactions(It.IsAny<long>(), false)).Returns(fakeLastTransactions);
            MockUserService(fakeBet?.UserId);

            var betService = new BetService(_httpContextAccessor.Object,
                                            _paymentOrderService.Object,
                                            _userService.Object,
                                            _emailSender.Object,
                                            _operationRecorder.Object,
                                            _sessionTracker.Object,
                                            _logger.Object,
                                            _appSettings.Object);

            //Act
            Task<Bet> settleWin() => betService.SettleWin(fakeNewBetRequest, fakeNewTransactions);

            //Assert
            var isDataMismatched = (fakeBet.BetStatusId == BetStatusEnum.Won || fakeBet.BetStatusId == BetStatusEnum.WonNotPaid) && //retry
                                    !fakeLastTransactions.CompareList(fakeNewTransactions);

            if (isDataMismatched)
            {
                var exception = await Assert.ThrowsAsync<IppicaException>(settleWin);
            }
            else
            {
                fakeBet = await settleWin();
                Assert.NotNull(fakeBet);
                Assert.Equal(Math.Abs(fakeNewWin.Amount ?? 0), Math.Abs(fakeBet.WinAmount ?? 0));
                Assert.Equal(Math.Abs(fakeNewStakeCompensation?.Amount ?? 0), Math.Abs(fakeBet.RefundAmount ?? 0));
                Assert.Equal(BetStatusEnum.Won, fakeBet.BetStatusId);
            }
        }

        [Theory]
        [InlineData(BetHelperAction.Create_Fix_Reserved)]
        [InlineData(BetHelperAction.Create_Fix_Placed)]
        [InlineData(BetHelperAction.Create_Fix_Lost)]
        [InlineData(BetHelperAction.Create_Psip_Won_IncludingStakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_UndoRefund)]
        public async Task SettleLost(BetHelperAction action)
        {
            //Arrange

            //current state
            var fakeHelper = new BetHelper(action);
            var fakeBet = fakeHelper.GetBet();

            //new data
            var fakeNewBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeBet, BetRequestTypeEnum.WebSettleBet));

            //mock
            _betRepository.Setup(x => x.GetBet(It.IsAny<long>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBetByExternalId(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            MockUserService(fakeBet?.UserId);

            var betService = new BetService(_httpContextAccessor.Object,
                                            _paymentOrderService.Object,
                                            _userService.Object,
                                            _emailSender.Object,
                                            _operationRecorder.Object,
                                            _sessionTracker.Object,
                                            _logger.Object,
                                            _appSettings.Object);

            //Act
            Task<Bet> settleLoss() => betService.SettleLoss(fakeNewBetRequest);

            //Assert
            var supportedStatuses = new List<BetStatusEnum>() { BetStatusEnum.Placed,
                                                                BetStatusEnum.Lost};
            var isStatusSupported = supportedStatuses.Exists(x => x == fakeBet.BetStatusId);

            if (!isStatusSupported)
            {
                var exception = await Assert.ThrowsAsync<IppicaException>(settleLoss);
            }
            else
            {
                fakeBet = await settleLoss();
                Assert.NotNull(fakeBet);
                Assert.Equal(0, Math.Abs(fakeBet.WinAmount ?? 0));
                Assert.Equal(0, Math.Abs(fakeBet.RefundAmount ?? 0));
                Assert.Equal(BetStatusEnum.Lost, fakeBet.BetStatusId);
            }
        }

        [Theory]
        [InlineData(BetHelperAction.Create_Fix_Reserved)]
        [InlineData(BetHelperAction.Create_Fix_Reserved, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Fix_Reserved, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_Refunded, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Placed, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Placed, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Refunded, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_UndoRefund, BetTransactionTypeEnum.RefundStake)]
        public async Task CancelStake(BetHelperAction action, params BetTransactionTypeEnum[] newTransactionTypes)
        {
            //Arrange

            //current state
            var fakeHelper = new BetHelper(action);
            var fakeBet = fakeHelper.GetBet();
            var fakeLastTransactions = fakeHelper.GetLastBetTransactions();
            var fakeStake = fakeHelper.GetBetTransaction(BetTransactionTypeEnum.Stake);

            //new data
            var fakeNewBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeBet, BetRequestTypeEnum.WebRollbackBet));
            var fakeNewTransactions = BetHelper.Clear(BetHelper.CreateBetTransactions(fakeBet, newTransactionTypes));
            var fakeNewRefundStake = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.RefundStake);
            var fakeNewStakeCompensation = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.StakeCompensation);

            //mock
            _betRepository.Setup(x => x.GetBet(It.IsAny<long>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBetByExternalId(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, false)).Returns(fakeStake);
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, true)).Returns(BetHelper.ConvertToRefund(fakeStake));
            _betRepository.Setup(x => x.GetLastBetTransactions(It.IsAny<long>(), false)).Returns(fakeLastTransactions);
            MockUserService(fakeBet?.UserId);

            var betService = new BetService(_httpContextAccessor.Object,
                                            _paymentOrderService.Object,
                                            _userService.Object,
                                            _emailSender.Object,
                                            _operationRecorder.Object,
                                            _sessionTracker.Object,
                                            _logger.Object,
                                            _appSettings.Object);

            //Act
            Task<Bet> cancelStake() => betService.CancelStake(fakeNewBetRequest, fakeNewTransactions);

            //Assert
            var supportedStatuses = new List<BetStatusEnum>() { BetStatusEnum.Reserved,
                                                                BetStatusEnum.Placed,
                                                                BetStatusEnum.Refunded,
                                                                BetStatusEnum.RefundedNotPaid };
            var supportedTransactions = new List<BetTransactionTypeEnum>() { BetTransactionTypeEnum.RefundStake,
                                                                             BetTransactionTypeEnum.RefundTaxStake,
                                                                             BetTransactionTypeEnum.StakeCompensation };
            var isDataMismatched = fakeBet.BetStatusId == BetStatusEnum.Refunded && !fakeLastTransactions.CompareList(fakeNewTransactions); //retry
            var isStatusSupported = supportedStatuses.Exists(x => x == fakeBet.BetStatusId);
            var containsUnsupportedTransaction = fakeNewTransactions.Exists(x => !supportedTransactions.Exists(y => y == x.BetTransactionTypeId));
            var isPartialRefundForFixBet = fakeBet.BetTypeId == BetTypeEnum.Fix && fakeNewStakeCompensation != null;

            if (isDataMismatched || !isStatusSupported || isPartialRefundForFixBet || containsUnsupportedTransaction)
            {
                var exception = await Assert.ThrowsAsync<IppicaException>(cancelStake);
            }
            else
            {
                fakeBet = await cancelStake();
                Assert.NotNull(fakeBet);
                Assert.Equal(0, Math.Abs(fakeBet.WinAmount ?? 0));
                var refundAmount = (fakeNewTransactions?.Count != 0) ? Math.Abs(fakeNewRefundStake?.Amount ?? 0) + Math.Abs(fakeNewStakeCompensation?.Amount ?? 0) : Math.Abs(fakeStake?.Amount ?? 0);
                Assert.Equal(refundAmount, Math.Abs(fakeBet.RefundAmount ?? 0));
                Assert.Equal(BetStatusEnum.Refunded, fakeBet.BetStatusId);
            }
        }

        [Theory]
        [InlineData(BetHelperAction.Create_Fix_Refunded, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Psip_Refunded, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Psip_Refunded_ByStakeCompensation, BetTransactionTypeEnum.StakeCompensation)]
        public async Task CancelStake_RetryWithDifferentSettlementId(BetHelperAction action, params BetTransactionTypeEnum[] newTransactionTypes)
        {
            //Arrange

            //current state
            var fakeHelper = new BetHelper(action);
            var fakeBet = fakeHelper.GetBet();
            var fakeLastTransactions = fakeHelper.GetLastBetTransactions();
            var fakeStake = fakeHelper.GetBetTransaction(BetTransactionTypeEnum.Stake);
            var fakeBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeBet, BetRequestTypeEnum.WebSettleBet));
            var fakeWebBetRequest = BetHelper.CreateWebBetRequest(fakeBetRequest, BetSettlementReasonEnum.Refund);
            fakeBetRequest.WebBetRequest = fakeWebBetRequest;

            //new data
            var fakeNewBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeBet, BetRequestTypeEnum.WebSettleBet));
            var fakeNewWebBetRequest = BetHelper.CreateWebBetRequest(fakeNewBetRequest, BetSettlementReasonEnum.Refund);
            fakeNewBetRequest.WebBetRequest = fakeNewWebBetRequest;
            if (fakeNewBetRequest.WebBetRequest.BetSettlementId == fakeBetRequest.WebBetRequest.BetSettlementId)
                fakeNewBetRequest.WebBetRequest.BetSettlementId = fakeBetRequest.WebBetRequest.BetSettlementId + 1;//must be different BetSettlementId
            var fakeNewTransactions = BetHelper.Clear(BetHelper.CreateBetTransactions(fakeBet, newTransactionTypes));
            var fakeNewRefundStake = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.RefundStake);
            var fakeNewStakeCompensation = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.StakeCompensation);

            //mock
            _betRepository.Setup(x => x.GetBet(It.IsAny<long>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBetByExternalId(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, false)).Returns(fakeStake);
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, true)).Returns(BetHelper.ConvertToRefund(fakeStake));
            _betRepository.Setup(x => x.GetLastBetTransactions(It.IsAny<long>(), false)).Returns(fakeLastTransactions);
            _betRepository.Setup(x => x.GetLastBetRequest(It.IsAny<long>())).Returns(fakeBetRequest);

            MockUserService(fakeBet?.UserId);

            var betService = new BetService(_httpContextAccessor.Object,
                                            _paymentOrderService.Object,
                                            _userService.Object,
                                            _emailSender.Object,
                                            _operationRecorder.Object,
                                            _sessionTracker.Object,
                                            _logger.Object,
                                            _appSettings.Object);

            //Act
            Task<Bet> cancelStake() => betService.CancelStake(fakeNewBetRequest, fakeNewTransactions);

            //Assert
            var exception = await Assert.ThrowsAsync<IppicaException>(cancelStake);
        }

        [Theory]
        [InlineData(BetHelperAction.Create_Fix_Refunded, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Psip_Refunded, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Psip_Refunded_ByStakeCompensation, BetTransactionTypeEnum.StakeCompensation)]
        public async Task CancelStake_Retry(BetHelperAction action, params BetTransactionTypeEnum[] newTransactionTypes)
        {
            //Arrange

            //current state
            var fakeHelper = new BetHelper(action);
            var fakeBet = fakeHelper.GetBet();
            var fakeLastTransactions = fakeHelper.GetLastBetTransactions();
            var fakeStake = fakeHelper.GetBetTransaction(BetTransactionTypeEnum.Stake);
            var fakeBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeBet, BetRequestTypeEnum.WebSettleBet));
            var fakeWebBetRequest = BetHelper.CreateWebBetRequest(fakeBetRequest, BetSettlementReasonEnum.Refund);
            fakeBetRequest.WebBetRequest = fakeWebBetRequest;

            //new data
            var fakeNewBetRequest = fakeBetRequest.Clone();
            var fakeNewTransactions = BetHelper.Clear(BetHelper.CreateBetTransactions(fakeBet, newTransactionTypes));
            var fakeNewRefundStake = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.RefundStake);
            var fakeNewStakeCompensation = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.StakeCompensation);


            //mock
            _betRepository.Setup(x => x.GetBet(It.IsAny<long>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBetByExternalId(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, false)).Returns(fakeStake);
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, true)).Returns(BetHelper.ConvertToRefund(fakeStake));
            _betRepository.Setup(x => x.GetLastBetTransactions(It.IsAny<long>(), false)).Returns(fakeLastTransactions);
            _betRepository.Setup(x => x.GetLastBetRequest(It.IsAny<long>())).Returns(fakeBetRequest);
            MockUserService(fakeBet?.UserId);

            var betService = new BetService(_httpContextAccessor.Object,
                                            _paymentOrderService.Object,
                                            _userService.Object,
                                            _emailSender.Object,
                                            _operationRecorder.Object,
                                            _sessionTracker.Object,
                                            _logger.Object,
                                            _appSettings.Object);

            //Act
            Task<Bet> cancelStake() => betService.CancelStake(fakeNewBetRequest, fakeNewTransactions);

            //Assert
            fakeBet = await cancelStake();
            Assert.NotNull(fakeBet);
            Assert.Equal(0, Math.Abs(fakeBet.WinAmount ?? 0));
            var refundAmount = (fakeNewTransactions?.Count != 0) ? Math.Abs(fakeNewRefundStake?.Amount ?? 0) + Math.Abs(fakeNewStakeCompensation?.Amount ?? 0) : Math.Abs(fakeStake?.Amount ?? 0);
            Assert.Equal(refundAmount, Math.Abs(fakeBet.RefundAmount ?? 0));
            Assert.Equal(BetStatusEnum.Refunded, fakeBet.BetStatusId);
        }

        [Theory]
        [InlineData(BetHelperAction.Create_Fix_Reserved)]
        [InlineData(BetHelperAction.Create_Fix_Reserved, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Fix_Reserved, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_Refunded, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Placed, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Placed, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Refunded, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_UndoRefund, BetTransactionTypeEnum.RefundStake)]
        public async Task CancelStakeButStakeDoesNotExist(BetHelperAction action, params BetTransactionTypeEnum[] newTransactionTypes)
        {
            //Arrange

            //current state
            var fakeHelper = new BetHelper(action);
            var fakeBet = fakeHelper.GetBet();
            var fakeLastTransactions = fakeHelper.GetLastBetTransactions();
            var fakeStake = fakeHelper.GetBetTransaction(BetTransactionTypeEnum.Stake);

            //new data
            var fakeNewBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeBet, BetRequestTypeEnum.WebRollbackBet));
            var fakeNewTransactions = BetHelper.Clear(BetHelper.CreateBetTransactions(fakeBet, newTransactionTypes));
            var fakeNewRefundStake = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.RefundStake);
            var fakeNewStakeCompensation = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.StakeCompensation);

            //mock
            _betRepository.Setup(x => x.GetBet(It.IsAny<long>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBetByExternalId(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, false)).Returns<BetTransaction>(null);
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, true)).Returns<BetTransaction>(null);
            _betRepository.Setup(x => x.GetLastBetTransactions(It.IsAny<long>(), false)).Returns<BetTransaction>(null);
            MockUserService(fakeBet?.UserId);

            var betService = new BetService(_httpContextAccessor.Object,
                                            _paymentOrderService.Object,
                                            _userService.Object,
                                            _emailSender.Object,
                                            _operationRecorder.Object,
                                            _sessionTracker.Object,
                                            _logger.Object,
                                            _appSettings.Object);

            //Act
            Task<Bet> cancelStake() => betService.CancelStake(fakeNewBetRequest, fakeNewTransactions);

            //Assert
            var supportedStatuses = new List<BetStatusEnum>() { BetStatusEnum.Reserved,
                                                                BetStatusEnum.Placed,
                                                                BetStatusEnum.Refunded,
                                                                BetStatusEnum.RefundedNotPaid };
            var supportedTransactions = new List<BetTransactionTypeEnum>() { BetTransactionTypeEnum.RefundStake,
                                                                             BetTransactionTypeEnum.RefundTaxStake,
                                                                             BetTransactionTypeEnum.StakeCompensation };
            var isDataMismatched = fakeBet.BetStatusId == BetStatusEnum.Refunded && !fakeLastTransactions.CompareList(fakeNewTransactions); //retry
            var isStatusSupported = supportedStatuses.Exists(x => x == fakeBet.BetStatusId);
            var containsUnsupportedTransaction = fakeNewTransactions.Exists(x => !supportedTransactions.Exists(y => y == x.BetTransactionTypeId));
            var isPartialRefundForFixBet = fakeBet.BetTypeId == BetTypeEnum.Fix && fakeNewStakeCompensation != null;
            var hasFakeNewTransactions = (fakeNewTransactions?.Count ?? 0) > 0;//when the list is passed it will force CancelStake to check the amounts and existence of transactions

            if (isDataMismatched || !isStatusSupported || isPartialRefundForFixBet || containsUnsupportedTransaction || hasFakeNewTransactions)
            {
                var exception = await Assert.ThrowsAsync<IppicaException>(cancelStake);
            }
            else
            {
                fakeBet = await cancelStake();
                Assert.NotNull(fakeBet);
                Assert.Equal(0, Math.Abs(fakeBet.WinAmount ?? 0));
                var refundAmount = (fakeNewTransactions?.Count != 0) ? Math.Abs(fakeNewRefundStake?.Amount ?? 0) + Math.Abs(fakeNewStakeCompensation?.Amount ?? 0) : Math.Abs(fakeStake?.Amount ?? 0);
                Assert.Equal(refundAmount, Math.Abs(fakeBet.RefundAmount ?? 0));
                Assert.Equal(BetStatusEnum.Refunded, fakeBet.BetStatusId);
            }
        }

        [Theory]
        [InlineData(BetHelperAction.Create_Fix_Reserved)]
        [InlineData(BetHelperAction.Create_Fix_Reserved, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Fix_Reserved, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_Refunded, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Placed, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Placed, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Refunded, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        public async Task CancelStakeNotPaid(BetHelperAction action, params BetTransactionTypeEnum[] newTransactionTypes)
        {
            //Arrange

            //current state
            var fakeHelper = new BetHelper(action);
            var fakeBet = fakeHelper.GetBet();
            var fakeLastTransactions = fakeHelper.GetLastBetTransactions();
            var fakeStake = fakeHelper.GetBetTransaction(BetTransactionTypeEnum.Stake);

            //new data
            var fakeNewBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeBet, BetRequestTypeEnum.WebRollbackBet));
            var fakeNewTransactions = BetHelper.Clear(BetHelper.CreateBetTransactions(fakeBet, newTransactionTypes));
            var fakeNewRefundStake = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.RefundStake);
            var fakeNewStakeCompensation = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.StakeCompensation);

            //mock
            _betRepository.Setup(x => x.GetBet(It.IsAny<long>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBetByExternalId(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, false)).Returns(fakeStake);
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.Stake, true)).Returns(BetHelper.ConvertToRefund(fakeStake));
            _betRepository.Setup(x => x.GetLastBetTransactions(It.IsAny<long>(), false)).Returns(fakeLastTransactions);
            MockUserService(fakeBet?.UserId);

            var betService = new BetService(_httpContextAccessor.Object,
                                            _paymentOrderService.Object,
                                            _userService.Object,
                                            _emailSender.Object,
                                            _operationRecorder.Object,
                                            _sessionTracker.Object,
                                            _logger.Object,
                                            _appSettings.Object);

            //Act
            Task<Bet> cancelStake() => betService.CancelStake(fakeNewBetRequest, fakeNewTransactions, true);

            //Assert
            var supportedStatuses = new List<BetStatusEnum>() { BetStatusEnum.Reserved,
                                                                BetStatusEnum.Placed,
                                                                BetStatusEnum.RefundedNotPaid };
            var supportedTransactions = new List<BetTransactionTypeEnum>() { BetTransactionTypeEnum.RefundStake,
                                                                             BetTransactionTypeEnum.RefundTaxStake,
                                                                             BetTransactionTypeEnum.StakeCompensation };
            var isDataMismatched = fakeBet.BetStatusId == BetStatusEnum.Refunded && !fakeLastTransactions.CompareList(fakeNewTransactions); //retry
            var isStatusSupported = supportedStatuses.Exists(x => x == fakeBet.BetStatusId);
            var containsUnsupportedTransaction = fakeNewTransactions.Exists(x => !supportedTransactions.Exists(y => y == x.BetTransactionTypeId));
            var isPartialRefundForFixBet = fakeBet.BetTypeId == BetTypeEnum.Fix && fakeNewStakeCompensation != null;

            if (isDataMismatched || !isStatusSupported || isPartialRefundForFixBet || containsUnsupportedTransaction)
            {
                var exception = await Assert.ThrowsAsync<IppicaException>(cancelStake);
            }
            else
            {
                fakeBet = await cancelStake();
                Assert.NotNull(fakeBet);
                Assert.Equal(0, Math.Abs(fakeBet.WinAmount ?? 0));
                var refundAmount = (fakeNewTransactions?.Count != 0) ? Math.Abs(fakeNewRefundStake?.Amount ?? 0) + Math.Abs(fakeNewStakeCompensation?.Amount ?? 0) : Math.Abs(fakeStake?.Amount ?? 0);
                Assert.Equal(refundAmount, Math.Abs(fakeBet.RefundAmount ?? 0));
                Assert.Equal(BetStatusEnum.RefundedNotPaid, fakeBet.BetStatusId);
            }
        }


        [Theory]
        [InlineData(BetHelperAction.Create_Fix_Reserved)]
        [InlineData(BetHelperAction.Create_Fix_Reserved, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Fix_Reserved, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Fix_Placed, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Fix_Refunded, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Fix_RefundedNotPaid, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.RefundStake)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Reserved, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Placed, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Placed, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Refunded, BetTransactionTypeEnum.RefundStake, BetTransactionTypeEnum.StakeCompensation)]
        [InlineData(BetHelperAction.Create_Psip_Refunded, BetTransactionTypeEnum.Stake)]
        [InlineData(BetHelperAction.Create_Psip_Refunded_ByStakeCompensation, BetTransactionTypeEnum.RefundStakeCompensation)]
        public async Task UndoCancelStake(BetHelperAction action, params BetTransactionTypeEnum[] newTransactionTypes)
        {
            //Arrange

            //current state
            var fakeHelper = new BetHelper(action);
            var fakeBet = fakeHelper.GetBet();
            var fakeLastTransactions = fakeHelper.GetLastBetTransactions();
            var fakeStakeCompensation = fakeHelper.GetBetTransaction(BetTransactionTypeEnum.StakeCompensation);

            //new data
            var fakeNewBetRequest = BetHelper.Clear(BetHelper.CreateBetRequest(fakeBet, BetRequestTypeEnum.WebSettleBet));
            var fakeNewTransactions = BetHelper.Clear(BetHelper.CreateBetTransactions(fakeBet, newTransactionTypes));
            var fakeNewStake = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.Stake);
            var fakeNewRefundStakeCompensation = fakeNewTransactions.Find(x => x.BetTransactionTypeId == BetTransactionTypeEnum.RefundStakeCompensation);

            //mock
            _betRepository.Setup(x => x.GetBet(It.IsAny<long>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBet(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetBetByExternalId(It.IsAny<string>(), It.IsAny<bool>())).Returns(fakeBet.Clone());
            _betRepository.Setup(x => x.GetActiveTransaction(It.IsAny<long>(), BetTransactionTypeEnum.StakeCompensation, true)).Returns(BetHelper.ConvertToRefund(fakeStakeCompensation));
            _betRepository.Setup(x => x.GetLastBetTransactions(It.IsAny<long>(), false)).Returns(fakeLastTransactions);
            _betRepository.Setup(x => x.GetLastBetTransactions(It.IsAny<long>(), true)).Returns(BetHelper.ConvertToRefund(fakeLastTransactions));
            MockUserService(fakeBet?.UserId);

            var betService = new BetService(_httpContextAccessor.Object,
                                            _paymentOrderService.Object,
                                            _userService.Object,
                                            _emailSender.Object,
                                            _operationRecorder.Object,
                                            _sessionTracker.Object,
                                            _logger.Object,
                                            _appSettings.Object);

            //Act
            Task<Bet> undoCancelStake() => betService.UndoCancelStake(fakeNewBetRequest, fakeNewTransactions);

            //Assert
            var supportedStatuses = new List<BetStatusEnum>() { BetStatusEnum.Placed,
                                                                BetStatusEnum.Refunded };
            var supportedTransactions = new List<BetTransactionTypeEnum>() { BetTransactionTypeEnum.Stake,
                                                                             BetTransactionTypeEnum.TaxStake,
                                                                             BetTransactionTypeEnum.RefundStakeCompensation };
            var isDataMismatched = fakeBet.BetStatusId == BetStatusEnum.Placed && !fakeLastTransactions.CompareList(fakeNewTransactions); //retry
            var isStatusSupported = supportedStatuses.Exists(x => x == fakeBet.BetStatusId);
            var containsUnsupportedTransaction = fakeNewTransactions.Exists(x => !supportedTransactions.Exists(y => y == x.BetTransactionTypeId));

            if (isDataMismatched || !isStatusSupported || containsUnsupportedTransaction)
            {
                var exception = await Assert.ThrowsAsync<IppicaException>(undoCancelStake);
            }
            else
            {
                fakeBet = await undoCancelStake();
                Assert.NotNull(fakeBet);
                Assert.Equal(0, Math.Abs(fakeBet.WinAmount ?? 0));
                Assert.Equal(0, Math.Abs(fakeBet.RefundAmount ?? 0));
                Assert.Equal(BetStatusEnum.Placed, fakeBet.BetStatusId);
            }
        }
    }
}


