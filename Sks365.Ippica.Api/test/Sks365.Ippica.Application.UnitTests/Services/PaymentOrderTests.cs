using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Sks365.Ippica.Application.Services;
using Sks365.Ippica.Application.Services.Abstraction;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.DataAccess;
using Sks365.Ippica.DataAccess.Repositories.Abstraction;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.Ippica.FakeData.Client;
using Sks365.Ippica.FakeData.Domain;
using Sks365.Payments.WebApi.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Sks365.Ippica.Application.UnitTests.Services
{
    public class PaymentOrderTests
    {
        private readonly Mock<IPaymentClient> _paymentClient;
        private readonly Mock<ITransactionClient> _transactionClient;
        private readonly Mock<IServiceProvider> _serviceProvider;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly Mock<IPaymentOrderRepository> _paymentOrderRepository;
        private readonly Mock<IIsbetsUnitOfWork> _isbetsUnitOfWork;
        private readonly Mock<IMstUnitOfWork> _mstUnitOfWork;
        private IPaymentOrderService _paymentOrderService;
        private Mock<ILogger<PaymentOrderService>> _logger;

        public PaymentOrderTests()
        {
            _paymentClient = new Mock<IPaymentClient>();
            _transactionClient = new Mock<ITransactionClient>();
            _serviceProvider = new Mock<IServiceProvider>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _isbetsUnitOfWork = new Mock<IIsbetsUnitOfWork>();
            _mstUnitOfWork = new Mock<IMstUnitOfWork>();
            _paymentOrderRepository = new Mock<IPaymentOrderRepository>();
            _logger = new Mock<ILogger<PaymentOrderService>>();

            _httpContextAccessor.SetupGet(x => x.HttpContext.RequestServices).Returns(_serviceProvider.Object);
            _serviceProvider.Setup(x => x.GetService(typeof(IIsbetsUnitOfWork))).Returns(_isbetsUnitOfWork.Object);
            _isbetsUnitOfWork.SetupGet(x => x.PaymentOrderRepository).Returns(_paymentOrderRepository.Object);
        }

        [Fact]
        public void Get_Payment_Order()
        {
            // Arrange
            var fakeOrder = new FakePaymentOrder().FakeData.Generate();
            _paymentOrderRepository.Setup(x => x.GetPaymentOrder(fakeOrder.PaymentOrderId.Value)).Returns(fakeOrder);
            _paymentOrderService = new PaymentOrderService(_paymentClient.Object, _transactionClient.Object, _httpContextAccessor.Object, _logger.Object);

            // Act
            var paymentOrder = _paymentOrderService.GetPaymentOrder(fakeOrder.PaymentOrderId.Value);

            // Assert
            Assert.Equal(fakeOrder, paymentOrder);
        }

        [Theory]
        [InlineData(100, PaymentOrderStatusEnum.ManuallyReversed)]
        [InlineData(-50, PaymentOrderStatusEnum.Pending)]
        [InlineData(100, PaymentOrderStatusEnum.ToBeProcessed)]
        [InlineData(-50, PaymentOrderStatusEnum.ToBeProcessed)]
        public async Task Initiate_Payment_Order(decimal amount, PaymentOrderStatusEnum paymentOrderStatus)
        {
            // Arrange
            var fakePaymentTransaction = new FakePaymentTransaction().FakeData
                .RuleFor(x => x.CurrencyCode, CurrencyEnum.EUR.ToString("G"))
                .RuleFor(x => x.Amount, amount)
                .Generate();

            var fakeOrder = new FakePaymentOrder().FakeData
                .RuleFor(x => x.Amount, fakePaymentTransaction.Amount)
                .RuleFor(x => x.CurrencyId, CurrencyEnum.EUR)
                .RuleFor(x => x.TransactionId, fakePaymentTransaction.TransactionId)
                .RuleFor(x => x.StatusId, paymentOrderStatus)
                .Generate();

            _paymentOrderRepository.Setup(x => x.GetPaymentOrder(It.IsAny<long>())).Returns(fakeOrder);

            _paymentClient.Setup(x => x.InitiateDepositAsync(It.IsAny<DepositRequest>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(fakePaymentTransaction);
            _paymentClient.Setup(x => x.RequestWithdrawalAsync(It.IsAny<WithdrawalRequest>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(fakePaymentTransaction);

            _paymentOrderService = new PaymentOrderService(_paymentClient.Object, _transactionClient.Object, _httpContextAccessor.Object, _logger.Object);

            // Act
            // Assert
            if (paymentOrderStatus != PaymentOrderStatusEnum.ToBeProcessed)
            {
                await Assert.ThrowsAsync<PaymentException>(() => _paymentOrderService.Initiate(fakePaymentTransaction.Amount, CurrencyEnum.EUR, fakePaymentTransaction.UserId, (BookmakerEnum)fakePaymentTransaction.BookmakerId, null));
            }
            else
            {
                var result = await _paymentOrderService.Initiate(fakePaymentTransaction.Amount, CurrencyEnum.EUR, fakePaymentTransaction.UserId, (BookmakerEnum)fakePaymentTransaction.BookmakerId, null);
                Assert.Equal(fakePaymentTransaction.Amount, result.Amount);
                Assert.Equal(fakePaymentTransaction.TransactionId, result.TransactionId);
            }
        }

        [Fact]
        public async Task Settle_Payment_Order()
        {
            // Arrange
            var fakePaymentTransaction = new FakePaymentTransaction().FakeData.Generate();
            var fakeOrder = new FakePaymentOrder().FakeData
                .RuleFor(x => x.TransactionId, fakePaymentTransaction.TransactionId)
                .RuleFor(x => x.StatusId, PaymentOrderStatusEnum.Done)
                .Generate();

            _paymentClient.Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentState>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(fakePaymentTransaction);
            _paymentOrderRepository.Setup(x => x.GetPaymentOrder(It.IsAny<long>())).Returns(fakeOrder);
            _paymentOrderService = new PaymentOrderService(_paymentClient.Object, _transactionClient.Object, _httpContextAccessor.Object, _logger.Object);

            // Act
            var result = await _paymentOrderService.Settle(fakePaymentTransaction.CorrelationTransactionId, fakePaymentTransaction.ThirdPartyId, fakePaymentTransaction.UserId, (BookmakerEnum)fakePaymentTransaction.BookmakerId, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakePaymentTransaction.TransactionId, result.TransactionId);
        }

        [Fact]
        public async Task Settle_Stake_Compensation()
        {
            // Arrange
            var fakePaymentTransaction = new FakePaymentTransaction().FakeData.Generate();
            var fakeOrder = new FakePaymentOrder().FakeData
                .RuleFor(x => x.StatusId, PaymentOrderStatusEnum.Done)
                .RuleFor(x => x.TransactionId, fakePaymentTransaction.TransactionId)
                .Generate();

            _paymentClient.Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentState>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(fakePaymentTransaction);
            _paymentOrderRepository.Setup(x => x.GetPaymentOrder(It.IsAny<long>())).Returns(fakeOrder);
            _paymentOrderService = new PaymentOrderService(_paymentClient.Object, _transactionClient.Object, _httpContextAccessor.Object, _logger.Object);

            // Act
            var result = await _paymentOrderService.SettleStakeCompensation(fakePaymentTransaction.CorrelationTransactionId, fakePaymentTransaction.ThirdPartyId, fakePaymentTransaction.UserId, (BookmakerEnum)fakePaymentTransaction.BookmakerId, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakePaymentTransaction.TransactionId, result.TransactionId);
        }

        [Theory]
        [InlineData(PaymentOrderStatusEnum.ToBeProcessed, -10)]
        [InlineData(PaymentOrderStatusEnum.Done, -10)]
        [InlineData(PaymentOrderStatusEnum.ToBeProcessed, 10)]
        [InlineData(PaymentOrderStatusEnum.ToBeVerified, -10)]
        [InlineData(PaymentOrderStatusEnum.Pending, -10)]
        [InlineData(PaymentOrderStatusEnum.DoneWithErrors, 10)]
        [InlineData(PaymentOrderStatusEnum.DoneWithErrors, 0)]
        public async Task Refund_Order(PaymentOrderStatusEnum status, decimal amount)
        {
            // Arrange
            var fakePaymentTransaction = new FakePaymentTransaction().FakeData
                .RuleFor(x => x.Amount, amount)
                .Generate();
            var fakeTransaction = new FakeTransaction().FakeData
                .RuleFor(x => x.TransactionId, fakePaymentTransaction.TransactionId)
                .RuleFor(x => x.Amount, amount)
                .Generate();
            var fakeOrder1 = new FakePaymentOrder().FakeData
                .RuleFor(x => x.TransactionId, fakePaymentTransaction.TransactionId)
                .RuleFor(x => x.Amount, amount)
                .RuleFor(x => x.StatusId, status)
                .Generate();
            var fakeOrder2 = new FakePaymentOrder().FakeData
                .RuleFor(x => x.TransactionId, fakePaymentTransaction.TransactionId)
                .RuleFor(x => x.Amount, amount)
                .RuleFor(x => x.StatusId, PaymentOrderStatusEnum.ManuallyReversed)
                .Generate();
            var fakeWithdrawalCancel = new FakeWithdrawalCancelResult().FakeData
                .RuleFor(x => x.RefundTransactionId, fakePaymentTransaction.TransactionId)
                .Generate();
            var fakeDepositRefund = new FakeDepositRefundResult().FakeData
                .RuleFor(x => x.RefundTransactionId, fakePaymentTransaction.TransactionId)
                .Generate();

            var fakeOrderQueue = new Queue<PaymentOrder>();
            fakeOrderQueue.Enqueue(fakeOrder1);
            fakeOrderQueue.Enqueue(fakeOrder2);

            _paymentOrderRepository.Setup(x => x.GetPaymentOrder(It.IsAny<long>())).Returns(fakeOrderQueue.Dequeue);
            _paymentOrderRepository.Setup(x => x.GetTransaction(It.IsAny<long>())).Returns(fakeTransaction);

            _paymentClient.Setup(x => x.CancelWithdrawalAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(fakeWithdrawalCancel);
            _paymentClient.Setup(x => x.RefundWithdrawalAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(fakeWithdrawalCancel);
            _paymentClient.Setup(x => x.RefundDepositAsync(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(fakeDepositRefund);

            _paymentOrderService = new PaymentOrderService(_paymentClient.Object, _transactionClient.Object, _httpContextAccessor.Object, _logger.Object);

            // Assert
            if (amount == 0)
            {
                await Assert.ThrowsAsync<PaymentException>(() => _paymentOrderService.Refund(fakePaymentTransaction.CorrelationTransactionId, fakePaymentTransaction.UserId, (BookmakerEnum)fakePaymentTransaction.BookmakerId, null));
            }
            else if (amount < 0 && !(status == PaymentOrderStatusEnum.ToBeProcessed || status == PaymentOrderStatusEnum.Done))
            {
                await Assert.ThrowsAsync<PaymentException>(() => _paymentOrderService.Refund(fakePaymentTransaction.CorrelationTransactionId, fakePaymentTransaction.UserId, (BookmakerEnum)fakePaymentTransaction.BookmakerId, null));
            }
            else
            {
                var result = await _paymentOrderService.Refund(fakePaymentTransaction.CorrelationTransactionId, fakePaymentTransaction.UserId, (BookmakerEnum)fakePaymentTransaction.BookmakerId, null);
                Assert.NotNull(result);
                Assert.Equal(fakePaymentTransaction.TransactionId, result.TransactionId);
                Assert.Equal(amount, result.Amount);
            }
        }
    }
}
