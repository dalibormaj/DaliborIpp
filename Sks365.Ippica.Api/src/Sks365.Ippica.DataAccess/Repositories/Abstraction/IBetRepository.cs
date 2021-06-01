using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System.Collections.Generic;

namespace Sks365.Ippica.DataAccess.Repositories.Abstraction
{
    public interface IBetRepository : IRepository
    {
        BetRequest SaveBetRequest(BetRequest betRequest);
        Bet SaveBet(Bet bet);
        BetTransaction SaveBetTransaction(BetTransaction betTransaction);
        WebBetRequest SaveWebBetRequest(WebBetRequest webBetrequest);
        ShopBetRequest SaveShopBetRequest(ShopBetRequest shopBetRequest);
        List<FixBetDetail> SaveFixBetDetails(List<FixBetDetail> betDetails);
        List<SysBetDetail> SaveSysBetDetails(List<SysBetDetail> betDetails);
        PsipBetDetail SavePsipBetDetail(PsipBetDetail psipBetDetail);
        List<PsipScommessa> SavePsipScommessa(List<PsipScommessa> psipScommessa);
        PsrBetDetail SavePsrBetDetail(PsrBetDetail psipBetDetail);
        PsrScommessa SavePsrScommessa(PsrScommessa psrScommessa);
        void InsertLogRequest(LogRequest logRequest);

        List<Bet> GetBets(int? userId = null, long? betId = null, string ticketId = "", string externalId = "", bool getDetails = true);
        Bet GetBet(string ticketId, bool getDetails = true);
        Bet GetBet(long betId, bool getDetails = true);
        Bet GetBetByExternalId(string externalId, bool getDetails = true);
        List<BetTransaction> GetBetTransactions(long? betTransactionId = null, long? betRequestId = null, long? betId = null, string ticketId = "", string externalId = "", BetTransactionTypeEnum? betTransactionTypeId = null);
        List<BetTransaction> GetLastBetTransactions(long betId, bool convertToRefund = false);
        BetTransaction GetBetTransaction(long betTransactionId);
        List<FixBetDetail> GetFixBetDetails(long betId);
        List<FixBetDetail> GetFixBetDetails(string ticketId);
        List<SysBetDetail> GetSysBetDetails(long betId);
        List<SysBetDetail> GetSysBetDetails(string ticketId);
        PsipBetDetail GetPsipBetDetails(long betId);
        PsipBetDetail GetPsipBetDetails(string ticketId);
        PsrBetDetail GetPsrBetDetails(long betId);
        PsrBetDetail GetPsrBetDetails(string ticketId);
        List<PsipScommessa> GetPsipScommessa(long psipBetDetailsId);
        List<PsrScommessa> GetPsrScommessa(long psrBetDetailsId);
        List<PsrScommessaGruppo> GetPsrScommessaGruppo(long psrScommessaId);
        List<PsrScommessaEvento> GetPsrScommessaEvento(long psrScommessaGruppoId);
        List<BetRequest> GetBetRequestsByBetId(long betId);
        BetRequest GetLastBetRequest(long betId);
        WebBetRequest GetWebBetRequest(long betRequestId);
        ShopBetRequest GetShopBetRequest(long betRequestId);
        BetTransaction GetActiveTransaction(long betId, BetTransactionTypeEnum betTransactionTypeId, bool convertToRefund = false);

        bool IsEmailSentToday(Email email);
        void InsertLogEmail(Email email);
    }
}
