using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using Sks365.Payments.WebApi.Client;
using System;

namespace Sks365.Ippica.Application.Utility
{
    public static class PaymentOrderFactory
    {
        public static PaymentOrder Create(PaymentTransaction paymentTransaction)
        {
            return new PaymentOrder()
            {
                PaymentOrderId = paymentTransaction.CorrelationTransactionId,
                TransactionId = paymentTransaction.TransactionId,
                Amount = paymentTransaction.Amount,
                CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), paymentTransaction.CurrencyCode),
                StatusId = paymentTransaction.State == PaymentStatus.Failed ? PaymentOrderStatusEnum.DoneWithErrors :
                           paymentTransaction.State == PaymentStatus.Initiated ? PaymentOrderStatusEnum.ToBeProcessed :
                           paymentTransaction.State == PaymentStatus.Pending ? PaymentOrderStatusEnum.Pending :
                           paymentTransaction.State == PaymentStatus.Succeeded ? PaymentOrderStatusEnum.Done : throw new IppicaException(ReturnCodeEnum.Unknown, "Financial transaction in unknown status")
            };
        }
    }
}
