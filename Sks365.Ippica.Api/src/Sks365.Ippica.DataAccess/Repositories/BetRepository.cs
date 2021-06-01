using AutoMapper;
using Dapper;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.DataAccess.Repositories.Abstraction;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Sks365.Ippica.DataAccess.Repositories
{
    public class BetRepository : IBetRepository
    {
        private readonly IDataContext _dataContext;

        public BetRepository(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public List<BetRequest> GetBetRequestsByBetId(long betId)
        {
            var pars = new DynamicParameters(new
            {
                @BetId = betId
            });

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, BetRequest>()
                .ForMember(d => d.BetRequestId, opt => opt.MapFrom(src => src["BetRequestId"]))
                .ForMember(d => d.BetRequestTypeId, opt => opt.MapFrom(src => src["BetRequestTypeId"]))
                .ForMember(d => d.BetRequestTypeId, opt => opt.MapFrom(src => src["BetRequestTypeId"]))
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src["IDUtente"]))
                .ForMember(d => d.Session, opt => opt.MapFrom(src => src["Session"]))
                .ForMember(d => d.TicketId, opt => opt.MapFrom(src => src["TicketId"]))
                .ForMember(d => d.Game, opt => opt.MapFrom(src => src["Game"]))
                .ForMember(d => d.Games, opt => opt.MapFrom(src => src["Games"]))
                .ForMember(d => d.BetId, opt => opt.MapFrom(src => src["BetId"]))
                .ForMember(d => d.Ip, opt => opt.MapFrom(src => src["Ip"]))
                .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => src["Timestamp"]));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<BetRequest>("dbo.GetBetRequests_ByBetId", mapper, parameters: pars);

            if ((res?.Count ?? 0) > 0)
            {
                foreach (var item in res)
                {
                    item.WebBetRequest = "Web".Equals(item.BetRequestTypeId.Value.GetDescription()) ? GetWebBetRequest((long)item.BetRequestId) : null;
                    item.ShopBetRequest = "Shop".Equals(item.BetRequestTypeId.Value.GetDescription()) ? GetShopBetRequest((long)item.BetRequestId) : null;
                }
            }

            return res;
        }

        public BetRequest GetLastBetRequest(long betId)
        {
            var pars = new DynamicParameters(new
            {
                BetId = betId
            });

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, BetRequest>()
                .ForMember(d => d.BetRequestId, opt => opt.MapFrom(src => src["BetRequestId"]))
                .ForMember(d => d.BetRequestTypeId, opt => opt.MapFrom(src => src["BetRequestTypeId"]))
                .ForMember(d => d.BetRequestTypeId, opt => opt.MapFrom(src => src["BetRequestTypeId"]))
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src["IDUtente"]))
                .ForMember(d => d.Session, opt => opt.MapFrom(src => src["Session"]))
                .ForMember(d => d.TicketId, opt => opt.MapFrom(src => src["TicketId"]))
                .ForMember(d => d.Game, opt => opt.MapFrom(src => src["Game"]))
                .ForMember(d => d.Games, opt => opt.MapFrom(src => src["Games"]))
                .ForMember(d => d.BetId, opt => opt.MapFrom(src => src["BetId"]))
                .ForMember(d => d.Ip, opt => opt.MapFrom(src => src["Ip"]))
                .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => src["Timestamp"]));
            }).CreateMapper();

            var res = _dataContext.ExecuteReaderProcedure<BetRequest>("dbo.GetLastBetRequest", mapper, parameters: pars).FirstOrDefault();

            res.WebBetRequest = "Web".Equals(res.BetRequestTypeId.Value.GetDescription()) ? GetWebBetRequest((long)res.BetRequestId) : null;
            res.ShopBetRequest = "Shop".Equals(res.BetRequestTypeId.Value.GetDescription()) ? GetShopBetRequest((long)res.BetRequestId) : null;

            return res;
        }

        public WebBetRequest GetWebBetRequest(long betRequestId)
        {
            var pars = new DynamicParameters(new
            {
                BetRequestId = betRequestId,
            });

            var res = _dataContext.ExecuteReaderProcedure<WebBetRequest>("dbo.GetWebBetRequest", parameters: pars).FirstOrDefault();

            return res;
        }

        public ShopBetRequest GetShopBetRequest(long betRequestId)
        {
            var pars = new DynamicParameters(new
            {
                BetRequestId = betRequestId,
            });

            var res = _dataContext.ExecuteReaderProcedure<ShopBetRequest>("dbo.GetShopBetRequest", parameters: pars).FirstOrDefault();

            return res;
        }

        public List<Bet> GetBets(int? userId = null, long? betId = null, string ticketId = "", string externalId = "", bool getDetails = true)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, Bet>()
                .ForMember(d => d.BetId, opt => opt.MapFrom(src => src["BetId"]))
                .ForMember(d => d.BetStatusId, opt => opt.MapFrom(src => src["BetStatusId"]))
                .ForMember(d => d.BetTypeId, opt => opt.MapFrom(src => src["BetTypeId"]))
                .ForMember(d => d.BookmakerId, opt => opt.MapFrom(src => src["BookmakerId"]))
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src["IDUtente"]))
                .ForMember(d => d.TicketId, opt => opt.MapFrom(src => src["TicketId"]))
                .ForMember(d => d.Amount, opt => opt.MapFrom(src => src["Amount"]))
                .ForMember(d => d.Stake, opt => opt.MapFrom(src => src["Stake"]))
                .ForMember(d => d.CurrencyId, opt => opt.MapFrom(src => Enum.Parse(typeof(CurrencyEnum), src["CurrencyCode"].ToString())))
                .ForMember(d => d.TaxStake, opt => opt.MapFrom(src => src["TaxStake"]))
                .ForMember(d => d.WinAmount, opt => opt.MapFrom(src => src["WinAmount"]))
                .ForMember(d => d.TaxWin, opt => opt.MapFrom(src => src["TaxWin"]))
                .ForMember(d => d.ExternalId, opt => opt.MapFrom(src => src["ExternalId"]))
                .ForMember(d => d.Emission, opt => opt.MapFrom(src => src["Emission"]))
                .ForMember(d => d.EmissionUtc, opt => opt.MapFrom(src => src["EmissionUtc"]))
                .ForMember(d => d.MaxWinning, opt => opt.MapFrom(src => src["MaxWinning"]))
                .ForMember(d => d.Bets, opt => opt.MapFrom(src => src["Bets"]))
                .ForMember(d => d.Competence, opt => opt.MapFrom(src => src["Competence"]))
                .ForMember(d => d.Type, opt => opt.MapFrom(src => src["Type"]))
                .ForMember(d => d.BonusId, opt => opt.MapFrom(src => src["BonusId"]))
                .ForMember(d => d.Bonus, opt => opt.MapFrom(src => src["Bonus"]))
                .ForMember(d => d.Source, opt => opt.MapFrom(src => src["Source"]))
                .ForMember(d => d.Antepost, opt => opt.MapFrom(src => src["Antepost"]))
                .ForMember(d => d.RefundAmount, opt => opt.MapFrom(src => src["RefundAmount"]));
            }).CreateMapper();

            var pars = new DynamicParameters(new
            {
                IDUtente = userId,
                BetId = betId,
                TicketId = ticketId,
                ExternalId = externalId,
            });

            var res = _dataContext.ExecuteReaderProcedure<Bet>("dbo.GetBets", mapper, parameters: pars);

            if (getDetails)
            {
                foreach (var item in res)
                {
                    item.FixBetDetails = (item.BetTypeId == BetTypeEnum.Fix) ? GetFixBetDetails((long)item.BetId) : null;
                    item.SysBetDetails = (item.BetTypeId == BetTypeEnum.System) ? GetSysBetDetails((long)item.BetId) : null;
                    item.PsipBetDetails = (item.BetTypeId == BetTypeEnum.PsipTote) ? GetPsipBetDetails((long)item.BetId) : null;
                    item.PsrBetDetails = (item.BetTypeId == BetTypeEnum.PsrTote) ? GetPsrBetDetails((long)item.BetId) : null;
                }
            }

            return res;
        }

        public Bet GetBet(string ticketId, bool getDetails = true)
        {
            var bets = GetBets(ticketId: ticketId, getDetails: getDetails);
            return bets?.FirstOrDefault();
        }

        public Bet GetBet(long betId, bool getDetails = true)
        {
            var bets = GetBets(betId: betId, getDetails: getDetails);
            return bets?.FirstOrDefault();
        }

        public Bet GetBetByExternalId(string externalId, bool getDetails = true)
        {
            var bets = GetBets(externalId: externalId, getDetails: getDetails);
            return bets?.FirstOrDefault();
        }

        public List<BetTransaction> GetBetTransactions(long? betTransactionId = null, long? betRequestId = null, long? betId = null, string ticketId = "", string externalId = "", BetTransactionTypeEnum? betTransactionTypeId = null)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, BetTransaction>()
                .ForMember(d => d.BetTransactionId, opt => opt.MapFrom(src => src["BetTransactionId"]))
                .ForMember(d => d.BetTransactionTypeId, opt => opt.MapFrom(src => src["BetTransactionTypeId"]))
                .ForMember(d => d.BetRequestId, opt => opt.MapFrom(src => src["BetRequestId"]))
                .ForMember(d => d.BetId, opt => opt.MapFrom(src => src["BetId"]))
                .ForMember(d => d.Amount, opt => opt.MapFrom(src => src["Amount"]))
                .ForMember(d => d.CurrencyCode, opt => opt.MapFrom(src => src["CurrencyCode"]))
                .ForMember(d => d.TransactionId, opt => opt.MapFrom(src => src["IDTransazione"]))
                .ForMember(d => d.PaymentOrderId, opt => opt.MapFrom(src => src["IDCorrelazioneTransazioni"]))
                .ForMember(d => d.RefundBetTransactionId, opt => opt.MapFrom(src => src["RefundBetTransactionId"]));
            }).CreateMapper();

            var pars = new DynamicParameters(new
            {
                BetTransactionId = betTransactionId,
                BetRequestId = betRequestId,
                BetId = betId,
                TicketId = ticketId,
                ExternalId = externalId,
                BetTransactionTypeId = betTransactionTypeId
            });

            var res = _dataContext.ExecuteReaderProcedure<BetTransaction>("dbo.GetBetTransactions", mapper, parameters: pars);

            return res;
        }

        public List<BetTransaction> GetLastBetTransactions(long betId, bool convertToRefund = false)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, BetTransaction>()
                .ForMember(d => d.BetTransactionId, opt => opt.MapFrom(src => src["BetTransactionId"]))
                .ForMember(d => d.BetTransactionTypeId, opt => opt.MapFrom(src => src["BetTransactionTypeId"]))
                .ForMember(d => d.BetRequestId, opt => opt.MapFrom(src => src["BetRequestId"]))
                .ForMember(d => d.BetId, opt => opt.MapFrom(src => src["BetId"]))
                .ForMember(d => d.Amount, opt => opt.MapFrom(src => src["Amount"]))
                .ForMember(d => d.CurrencyCode, opt => opt.MapFrom(src => src["CurrencyCode"]))
                .ForMember(d => d.TransactionId, opt => opt.MapFrom(src => src["IDTransazione"]))
                .ForMember(d => d.PaymentOrderId, opt => opt.MapFrom(src => src["IDCorrelazioneTransazioni"]))
                .ForMember(d => d.RefundBetTransactionId, opt => opt.MapFrom(src => src["RefundBetTransactionId"]));
            }).CreateMapper();

            var pars = new DynamicParameters(new
            {
                BetId = betId,
                ConvertToRefund = convertToRefund
            });

            var res = _dataContext.ExecuteReaderProcedure<BetTransaction>("dbo.GetLastBetTransactions", mapper, parameters: pars);

            return res;
        }

        public BetTransaction GetBetTransaction(long betTransactionId)
        {
            var betTransactions = GetBetTransactions(betTransactionId);
            return betTransactions.FirstOrDefault();
        }


        public List<FixBetDetail> GetFixBetDetails(long betId)
        {
            var pars = new DynamicParameters(new
            {
                BetId = betId
            });

            var res = _dataContext.ExecuteReaderProcedure<FixBetDetail>("dbo.GetFixBetDetails", parameters: pars);

            return res;
        }

        public List<FixBetDetail> GetFixBetDetails(string ticketId)
        {
            var pars = new DynamicParameters(new
            {
                TicketId = ticketId
            });

            var res = _dataContext.ExecuteReaderProcedure<FixBetDetail>("dbo.GetFixBetDetails", parameters: pars);

            return res;
        }

        public List<SysBetDetail> GetSysBetDetails(long betId)
        {
            var pars = new DynamicParameters(new
            {
                BetId = betId
            });

            var res = _dataContext.ExecuteReaderProcedure<SysBetDetail>("dbo.GetSysBetDetails", parameters: pars);

            return res;
        }

        public List<SysBetDetail> GetSysBetDetails(string ticketId)
        {
            var pars = new DynamicParameters(new
            {
                TicketId = ticketId
            });

            var res = _dataContext.ExecuteReaderProcedure<SysBetDetail>("dbo.GetSysBetDetails", parameters: pars);

            return res;
        }

        public PsipBetDetail GetPsipBetDetails(long betId)
        {
            var pars = new DynamicParameters(new
            {
                BetId = betId
            });

            var res = _dataContext.ExecuteReaderProcedure<PsipBetDetail>("dbo.GetPsipBetDetails", parameters: pars).FirstOrDefault();

            if ((res?.PsipBetDetailsId ?? 0) > 0)
            {
                res.Scommessa = GetPsipScommessa((long)res.PsipBetDetailsId);
            }
            return res;
        }

        public PsipBetDetail GetPsipBetDetails(string ticketId)
        {
            var pars = new DynamicParameters(new
            {
                TicketId = ticketId
            });

            var res = _dataContext.ExecuteReaderProcedure<PsipBetDetail>("dbo.GetPsipBetDetails", parameters: pars).FirstOrDefault();

            if ((res?.PsipBetDetailsId ?? 0) > 0)
            {
                res.Scommessa = GetPsipScommessa((long)res.PsipBetDetailsId);
            }
            return res;
        }

        public PsrBetDetail GetPsrBetDetails(long betId)
        {
            var pars = new DynamicParameters(new
            {
                BetId = betId
            });

            var res = _dataContext.ExecuteReaderProcedure<PsrBetDetail>("dbo.GetPsrBetDetails", parameters: pars).FirstOrDefault();

            if ((res?.PsrBetDetailsId ?? 0) > 0)
            {
                res.Scommessa = GetPsrScommessa((long)res.PsrBetDetailsId);
            }

            return res;
        }

        public PsrBetDetail GetPsrBetDetails(string ticketId)
        {
            var pars = new DynamicParameters(new
            {
                TicketId = ticketId
            });

            var res = _dataContext.ExecuteReaderProcedure<PsrBetDetail>("dbo.GetPsrBetDetails", parameters: pars).FirstOrDefault();

            if ((res?.PsrBetDetailsId ?? 0) > 0)
            {
                res.Scommessa = GetPsrScommessa((long)res.PsrBetDetailsId);
            }

            return res;
        }

        public List<PsipScommessa> GetPsipScommessa(long psipBetDetailsId)
        {
            var pars = new DynamicParameters(new
            {
                PsipBetDetailsId = psipBetDetailsId
            });

            var res = _dataContext.ExecuteReaderProcedure<PsipScommessa>("dbo.GetPsipScommessa", parameters: pars);

            return res;
        }

        public List<PsrScommessa> GetPsrScommessa(long psrBetDetailsId)
        {
            var pars = new DynamicParameters(new
            {
                PsrBetDetailsId = psrBetDetailsId
            });

            var res = _dataContext.ExecuteReaderProcedure<PsrScommessa>("dbo.GetPsrScommessa", parameters: pars);

            foreach (var item in res)
            {
                item.Gruppo = GetPsrScommessaGruppo((long)item.PsrScommessaId);
            }

            return res;
        }

        public List<PsrScommessaGruppo> GetPsrScommessaGruppo(long psrScommessaId)
        {
            var pars = new DynamicParameters(new
            {
                PsrScommessaId = psrScommessaId
            });

            var res = _dataContext.ExecuteReaderProcedure<PsrScommessaGruppo>("dbo.GetPsrScommessaGruppo", parameters: pars);

            foreach (var item in res)
            {
                item.Evento = GetPsrScommessaEvento((long)item.PsrScommessaGruppoId);
            }

            return res;
        }

        public List<PsrScommessaEvento> GetPsrScommessaEvento(long psrScommessaGruppoId)
        {
            var pars = new DynamicParameters(new
            {
                PsrScommessaGruppoId = psrScommessaGruppoId
            });

            var res = _dataContext.ExecuteReaderProcedure<PsrScommessaEvento>("dbo.GetPsrScommessaEvento", parameters: pars);

            return res;
        }

        public Bet SaveBet(Bet bet)
        {

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, Bet>()
                .ForMember(d => d.BetId, opt => opt.MapFrom(src => src["BetId"]))
                .ForMember(d => d.BetStatusId, opt => opt.MapFrom(src => src["BetStatusId"]))
                .ForMember(d => d.BetTypeId, opt => opt.MapFrom(src => src["BetTypeId"]))
                .ForMember(d => d.BookmakerId, opt => opt.MapFrom(src => src["BookmakerId"]))
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src["IDUtente"]))
                .ForMember(d => d.TicketId, opt => opt.MapFrom(src => src["TicketId"]))
                .ForMember(d => d.Amount, opt => opt.MapFrom(src => src["Amount"]))
                .ForMember(d => d.Stake, opt => opt.MapFrom(src => src["Stake"]))
                .ForMember(d => d.CurrencyId, opt => opt.MapFrom(src => Enum.Parse(typeof(CurrencyEnum), src["CurrencyCode"].ToString())))
                .ForMember(d => d.TaxStake, opt => opt.MapFrom(src => src["TaxStake"]))
                .ForMember(d => d.WinAmount, opt => opt.MapFrom(src => src["WinAmount"]))
                .ForMember(d => d.TaxWin, opt => opt.MapFrom(src => src["TaxWin"]))
                .ForMember(d => d.ExternalId, opt => opt.MapFrom(src => src["ExternalId"]))
                .ForMember(d => d.Emission, opt => opt.MapFrom(src => src["Emission"]))
                .ForMember(d => d.EmissionUtc, opt => opt.MapFrom(src => src["EmissionUtc"]))
                .ForMember(d => d.MaxWinning, opt => opt.MapFrom(src => src["MaxWinning"]))
                .ForMember(d => d.Bets, opt => opt.MapFrom(src => src["Bets"]))
                .ForMember(d => d.Competence, opt => opt.MapFrom(src => src["Competence"]))
                .ForMember(d => d.Type, opt => opt.MapFrom(src => src["Type"]))
                .ForMember(d => d.BonusId, opt => opt.MapFrom(src => src["BonusId"]))
                .ForMember(d => d.Bonus, opt => opt.MapFrom(src => src["Bonus"]))
                .ForMember(d => d.Source, opt => opt.MapFrom(src => src["Source"]))
                .ForMember(d => d.Antepost, opt => opt.MapFrom(src => src["Antepost"]))
                .ForMember(d => d.RefundAmount, opt => opt.MapFrom(src => src["RefundAmount"]));
            }).CreateMapper();

            var pars = new DynamicParameters(new
            {
                BetId = bet.BetId,
                BetStatusId = bet.BetStatusId,
                BetTypeId = bet.BetTypeId,
                BookmakerId = bet.BookmakerId,
                IdUtente = bet.UserId,
                TicketId = bet.TicketId,
                Amount = bet.Amount,
                Stake = bet.Stake,
                CurrencyCode = Enum.GetName(typeof(CurrencyEnum), bet.CurrencyId),
                TaxStake = bet.TaxStake,
                WinAmount = bet.WinAmount,
                TaxWin = bet.TaxWin,
                ExternalId = bet.ExternalId,
                Emission = bet.Emission,
                EmissionUtc = bet.EmissionUtc,
                MaxWinning = bet.MaxWinning,
                Bets = bet.Bets,
                Competence = bet.Competence,
                Type = bet.Type,
                BonusId = bet.BonusId,
                Bonus = bet.Bonus,
                Source = bet.Source,
                Antepost = bet.Antepost,
                RefundAmount = bet.RefundAmount
            });

            var res = _dataContext.ExecuteReaderProcedure<Bet>("dbo.SaveBet", mapper, parameters: pars).FirstOrDefault();

            //Save BetDetails
            if ((res.BetId ?? 0) > 0 && (bet.FixBetDetails?.Count() ?? 0) > 0)
            {
                foreach (var item in bet.FixBetDetails) item.BetId = res.BetId;
                res.FixBetDetails = SaveFixBetDetails(bet.FixBetDetails).ToList();
            }

            if ((res.BetId ?? 0) > 0 && (bet.SysBetDetails?.Count() ?? 0) > 0)
            {
                foreach (var item in bet.SysBetDetails) item.BetId = res.BetId;
                res.SysBetDetails = SaveSysBetDetails(bet.SysBetDetails).ToList();
            }

            if ((res.BetId ?? 0) > 0 && (bet.PsipBetDetails != null))
            {
                bet.PsipBetDetails.BetId = res.BetId;
                res.PsipBetDetails = SavePsipBetDetail(bet.PsipBetDetails);
            }

            if ((res.BetId ?? 0) > 0 && (bet.PsrBetDetails != null))
            {
                bet.PsrBetDetails.BetId = res.BetId;
                res.PsrBetDetails = SavePsrBetDetail(bet.PsrBetDetails);
            }

            return res;

        }

        public BetTransaction SaveBetTransaction(BetTransaction betTransaction)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, BetTransaction>()
                .ForMember(d => d.BetTransactionId, opt => opt.MapFrom(src => src["BetTransactionId"]))
                .ForMember(d => d.BetTransactionTypeId, opt => opt.MapFrom(src => src["BetTransactionTypeId"]))
                .ForMember(d => d.BetRequestId, opt => opt.MapFrom(src => src["BetRequestId"]))
                .ForMember(d => d.BetId, opt => opt.MapFrom(src => src["BetId"]))
                .ForMember(d => d.Amount, opt => opt.MapFrom(src => src["Amount"]))
                .ForMember(d => d.CurrencyCode, opt => opt.MapFrom(src => src["CurrencyCode"]))
                .ForMember(d => d.TransactionId, opt => opt.MapFrom(src => src["IDTransazione"]))
                .ForMember(d => d.PaymentOrderId, opt => opt.MapFrom(src => src["IDCorrelazioneTransazioni"]))
                .ForMember(d => d.RefundBetTransactionId, opt => opt.MapFrom(src => src["RefundBetTransactionId"]));
            }).CreateMapper();

            var pars = new DynamicParameters(new
            {
                betTransaction.BetTransactionId,
                betTransaction.BetTransactionTypeId,
                betTransaction.BetRequestId,
                betTransaction.BetId,
                betTransaction.Amount,
                betTransaction.CurrencyCode,
                IDTransazione = betTransaction.TransactionId,
                IDCorrelazioneTransazioni = betTransaction.PaymentOrderId,
                betTransaction.RefundBetTransactionId
            });

            var res = _dataContext.ExecuteReaderProcedure<BetTransaction>("dbo.SaveBetTransaction", mapper, parameters: pars).FirstOrDefault();

            return res;
        }

        public List<FixBetDetail> SaveFixBetDetails(List<FixBetDetail> betDetails)
        {
            DataTable dtBetDetails = new DataTable("FixBetDetails");
            dtBetDetails.Columns.Add("FixBetDetailsId", typeof(long));
            dtBetDetails.Columns.Add("BetId", typeof(long));
            dtBetDetails.Columns.Add("Race", typeof(int));
            dtBetDetails.Columns.Add("Palinsesto", typeof(int));
            dtBetDetails.Columns.Add("Avvenimento", typeof(int));
            dtBetDetails.Columns.Add("Course");
            dtBetDetails.Columns.Add("Sigla");
            dtBetDetails.Columns.Add("Number", typeof(int));
            dtBetDetails.Columns.Add("RDate", typeof(DateTime));
            dtBetDetails.Columns.Add("Code", typeof(int));
            dtBetDetails.Columns.Add("Market");
            dtBetDetails.Columns.Add("InfoAggDes");
            dtBetDetails.Columns.Add("Horse", typeof(int));
            dtBetDetails.Columns.Add("Odd", typeof(decimal));
            dtBetDetails.Columns.Add("InfoAgg");
            dtBetDetails.Columns.Add("Country");
            dtBetDetails.Columns.Add("Des");

            foreach (FixBetDetail bd in betDetails)
            {
                DataRow drBetDetail = dtBetDetails.NewRow();
                drBetDetail["FixBetDetailsId"] = bd.FixBetDetailsId.HasValue ? bd.FixBetDetailsId : (object)DBNull.Value;
                drBetDetail["BetId"] = bd.BetId.HasValue ? bd.BetId : (object)DBNull.Value;
                drBetDetail["Race"] = bd.Race.HasValue ? bd.Race : (object)DBNull.Value;
                drBetDetail["Palinsesto"] = bd.Palinsesto.HasValue ? bd.Palinsesto : (object)DBNull.Value;
                drBetDetail["Avvenimento"] = bd.Avvenimento.HasValue ? bd.Avvenimento : (object)DBNull.Value;
                drBetDetail["Course"] = bd.Course;
                drBetDetail["Sigla"] = bd.Sigla;
                drBetDetail["Number"] = bd.Number.HasValue ? bd.Number : (object)DBNull.Value;
                drBetDetail["RDate"] = bd.RDate.HasValue ? bd.RDate : (object)DBNull.Value;
                drBetDetail["Code"] = bd.Code.HasValue ? bd.Code : (object)DBNull.Value;
                drBetDetail["Market"] = bd.Market;
                drBetDetail["InfoAggDes"] = bd.InfoAggDes;
                drBetDetail["Horse"] = bd.Horse.HasValue ? bd.Horse : (object)DBNull.Value;
                drBetDetail["Odd"] = bd.Odd.HasValue ? bd.Odd : (object)DBNull.Value;
                drBetDetail["InfoAgg"] = bd.InfoAgg;
                drBetDetail["Country"] = bd.Country;
                drBetDetail["Des"] = bd.Des;

                dtBetDetails.Rows.Add(drBetDetail);
            }

            var pars = new DynamicParameters(new
            {
                FixBetDetails = dtBetDetails
            });

            var res = _dataContext.ExecuteReaderProcedure<FixBetDetail>("dbo.SaveFixBetDetails", parameters: pars);
            return res;

        }

        public List<SysBetDetail> SaveSysBetDetails(List<SysBetDetail> betDetails)
        {
            DataTable table = new DataTable("SysBetDetails");
            table.Columns.Add("SysBetDetailsId", typeof(long));
            table.Columns.Add("BetId", typeof(long));
            table.Columns.Add("Race", typeof(int));
            table.Columns.Add("Palinsesto", typeof(int));
            table.Columns.Add("Avvenimento", typeof(int));
            table.Columns.Add("Course");
            table.Columns.Add("Number", typeof(int));
            table.Columns.Add("RDate", typeof(DateTime));
            table.Columns.Add("Code", typeof(int));
            table.Columns.Add("Market");
            table.Columns.Add("InfoAggDes");
            table.Columns.Add("Horse", typeof(int));
            table.Columns.Add("Odd", typeof(decimal));
            table.Columns.Add("System");
            table.Columns.Add("InfoAgg");
            table.Columns.Add("NumCombs");

            foreach (SysBetDetail bd in betDetails)
            {
                DataRow row = table.NewRow();
                row["SysBetDetailsId"] = bd.SysBetDetailsId.HasValue ? bd.SysBetDetailsId : (object)DBNull.Value;
                row["BetId"] = bd.BetId.HasValue ? bd.BetId : (object)DBNull.Value;
                row["Race"] = bd.Race.HasValue ? bd.Race : (object)DBNull.Value;
                row["Palinsesto"] = bd.Palinsesto.HasValue ? bd.Palinsesto : (object)DBNull.Value;
                row["Avvenimento"] = bd.Avvenimento.HasValue ? bd.Avvenimento : (object)DBNull.Value;
                row["Course"] = bd.Course;
                row["Number"] = bd.Number.HasValue ? bd.Number : (object)DBNull.Value;
                row["RDate"] = bd.RDate.HasValue ? bd.RDate : (object)DBNull.Value;
                row["Code"] = bd.Code.HasValue ? bd.Code : (object)DBNull.Value;
                row["Market"] = bd.Market;
                row["InfoAggDes"] = bd.InfoAggDes;
                row["Horse"] = bd.Horse.HasValue ? bd.Horse : (object)DBNull.Value;
                row["Odd"] = bd.Odd.HasValue ? bd.Odd : (object)DBNull.Value;
                row["System"] = bd.System;
                row["InfoAgg"] = bd.InfoAgg;
                row["NumCombs"] = bd.NumCombs.HasValue ? bd.NumCombs : (object)DBNull.Value;

                table.Rows.Add(row);
            }

            var pars = new DynamicParameters(new
            {
                SysBetDetails = table
            });

            var res = _dataContext.ExecuteReaderProcedure<SysBetDetail>("dbo.SaveSysBetDetails", parameters: pars);
            return res;
        }


        public PsipBetDetail SavePsipBetDetail(PsipBetDetail psipBetDetail)
        {
            var pars = new DynamicParameters(new
            {
                PsipBetDetailsId = psipBetDetail.PsipBetDetailsId,
                BetId = psipBetDetail.BetId,
                Fsc = psipBetDetail.Fsc,
                Conc = psipBetDetail.Conc,
                Pvend = psipBetDetail.Pvend,
                Terminale = psipBetDetail.Terminale,
                TipoGiocata = psipBetDetail.TipoGiocata,
                Sezione = psipBetDetail.Sezione,
                GiocataBonus = psipBetDetail.GiocataBonus,
                Race = psipBetDetail.Race,
                Palinsesto = psipBetDetail.Palinsesto,
                Avvenimento = psipBetDetail.Avvenimento,
                IdGiocata = psipBetDetail.IdGiocata
            });

            var res = _dataContext.ExecuteReaderProcedure<PsipBetDetail>("dbo.SavePsipBetDetails", parameters: pars).FirstOrDefault();

            if ((res.PsipBetDetailsId ?? 0) > 0 && psipBetDetail.Scommessa != null)
            {
                foreach (var item in psipBetDetail.Scommessa)
                {
                    item.PsipBetDetailsId = res.PsipBetDetailsId;
                    item.BetId = res.BetId;
                }
                res.Scommessa = SavePsipScommessa(psipBetDetail.Scommessa).ToList();
            }

            return res;
        }

        public List<PsipScommessa> SavePsipScommessa(List<PsipScommessa> psrScommessa)
        {
            DataTable table = new DataTable("PsipScommessa");
            table.Columns.Add("PsipScommessaId");
            table.Columns.Add("PsipBetDetailsId");
            table.Columns.Add("BetId");
            table.Columns.Add("Codice");
            table.Columns.Add("Importo");
            table.Columns.Add("Sistema");
            table.Columns.Add("Moltiplicatore");
            table.Columns.Add("Mappa");

            foreach (var item in psrScommessa)
            {
                DataRow row = table.NewRow();
                row["PsipScommessaId"] = item.PsipScommessaId.HasValue ? item.PsipScommessaId : (object)DBNull.Value;
                row["PsipBetDetailsId"] = item.PsipBetDetailsId.HasValue ? item.PsipBetDetailsId : (object)DBNull.Value;
                row["BetId"] = item.BetId.HasValue ? item.BetId : (object)DBNull.Value;
                row["Codice"] = item.Codice.HasValue ? item.Codice : (object)DBNull.Value;
                row["Importo"] = item.Importo.HasValue ? item.Importo : (object)DBNull.Value;
                row["Sistema"] = item.Sistema.HasValue ? item.Sistema : (object)DBNull.Value;
                row["Moltiplicatore"] = item.Moltiplicatore.HasValue ? item.Moltiplicatore : (object)DBNull.Value;
                row["Mappa"] = item.Mappa;

                table.Rows.Add(row);
            }

            var pars = new DynamicParameters(new
            {
                PsipScommessa = table
            });

            var res = _dataContext.ExecuteReaderProcedure<PsipScommessa>("dbo.SavePsipScommessa", parameters: pars);

            return res;
        }

        public PsrBetDetail SavePsrBetDetail(PsrBetDetail psrBetDetail)
        {
            var pars = new DynamicParameters(new
            {
                PsrBetDetailsId = psrBetDetail.PsrBetDetailsId,
                BetId = psrBetDetail.BetId,
                Fsc = psrBetDetail.Fsc,
                Conc = psrBetDetail.Conc,
                Pvend = psrBetDetail.Pvend,
                Terminale = psrBetDetail.Terminale,
                TipoGiocata = psrBetDetail.TipoGiocata,
                Sezione = psrBetDetail.Sezione,
                GiocataBonus = psrBetDetail.GiocataBonus,
                Race = psrBetDetail.Race,
                Palinsesto = psrBetDetail.Palinsesto,
                Avvenimento = psrBetDetail.Avvenimento,
                Concorso = psrBetDetail.Concorso,
                Tipo = psrBetDetail.Tipo,
                TipoConcorso = psrBetDetail.TipoConcorso,
                IdGiocata = psrBetDetail.IdGiocata,
                TipoCaratura = psrBetDetail.TipoCaratura,
                NumCarature = psrBetDetail.NumCarature
            });

            var res = _dataContext.ExecuteReaderProcedure<PsrBetDetail>("dbo.SavePsrBetDetails", parameters: pars).FirstOrDefault();

            if ((res.PsrBetDetailsId ?? 0) > 0 && (psrBetDetail?.Scommessa?.Count ?? 0) > 0)
            {
                res.Scommessa = res.Scommessa ?? new List<PsrScommessa>();

                foreach (var item in psrBetDetail.Scommessa)
                {
                    item.PsrBetDetailsId = res.PsrBetDetailsId;
                    item.BetId = res.BetId;
                    res.Scommessa.Add(SavePsrScommessa(item));
                }
            }

            return res;
        }

        public PsrScommessa SavePsrScommessa(PsrScommessa psrScommessa)
        {
            var pars = new DynamicParameters(new
            {
                PsrScommessaId = psrScommessa.PsrScommessaId,
                PsrBetDetailsId = psrScommessa.PsrBetDetailsId,
                BetId = psrScommessa.BetId,
                ImportoScommessa = psrScommessa.ImportoScommessa,
                Unita = psrScommessa.Unita,
                Moltiplicatore = psrScommessa.Moltiplicatore
            });

            var res = _dataContext.ExecuteReaderProcedure<PsrScommessa>("dbo.SavePsrScommessa", parameters: pars).FirstOrDefault();

            if ((res.PsrScommessaId ?? 0) > 0 && (psrScommessa.Gruppo?.Count ?? 0) > 0)
            {
                res.Gruppo = res.Gruppo ?? new List<PsrScommessaGruppo>();

                foreach (var item in psrScommessa.Gruppo)
                {
                    item.PsrScommessaId = res.PsrScommessaId;
                    item.BetId = res.BetId;
                    res.Gruppo.Add(SavePsrScommessaGruppo(item));
                }
            }

            return res;
        }

        public PsrScommessaGruppo SavePsrScommessaGruppo(PsrScommessaGruppo psrScommessaGruppo)
        {
            var pars = new DynamicParameters(new
            {
                PsrScommessaGruppoId = psrScommessaGruppo.PsrScommessaGruppoId,
                PsrScommessaId = psrScommessaGruppo.PsrScommessaId,
                BetId = psrScommessaGruppo.BetId,
                Codice = psrScommessaGruppo.Codice
            });

            var res = _dataContext.ExecuteReaderProcedure<PsrScommessaGruppo>("dbo.SavePsrScommessaGruppo", parameters: pars).FirstOrDefault();

            if ((res.PsrScommessaGruppoId ?? 0) > 0 && (psrScommessaGruppo.Evento?.Count ?? 0) > 0)
            {
                res.Evento = res.Evento ?? new List<PsrScommessaEvento>();

                foreach (var item in psrScommessaGruppo.Evento)
                {
                    item.PsrScommessaGruppoId = res.PsrScommessaGruppoId;
                    item.BetId = res.BetId;
                    res.Evento.Add(SavePsrScommessaEvento(item));
                }
            }

            return res;
        }

        public PsrScommessaEvento SavePsrScommessaEvento(PsrScommessaEvento psrScommessaEvento)
        {
            var pars = new DynamicParameters(new
            {
                PsrScommessaEventoId = psrScommessaEvento.PsrScommessaEventoId,
                PsrScommessaGruppoId = psrScommessaEvento.PsrScommessaGruppoId,
                BetId = psrScommessaEvento.BetId,
                Codice = psrScommessaEvento.Codice,
                Scom = psrScommessaEvento.Scom,
                Sistema = psrScommessaEvento.Sistema,
                Mappa = psrScommessaEvento.Mappa,
            });

            var res = _dataContext.ExecuteReaderProcedure<PsrScommessaEvento>("dbo.SavePsrScommessaEvento", parameters: pars).FirstOrDefault();

            return res;
        }

        public void InsertLogRequest(LogRequest logRequest)
        {
            var pars = new DynamicParameters(new
            {
                logRequest.Endpoint,
                logRequest.HttpMethod,
                logRequest.RequestBody,
                logRequest.ResponseBody,
                logRequest.HttpStatusCode,
                logRequest.Session,
                logRequest.TicketId,
                logRequest.ExternalId,
                logRequest.UserAccount,
                logRequest.RemoteIpAddress,
                logRequest.RequestDate,
                logRequest.ResponseDate
            });

            var res = _dataContext.ExecuteProcedure("dbo.InsertLogRequest", parameters: pars);
        }

        public BetRequest SaveBetRequest(BetRequest betRequest)
        {

            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, BetRequest>()
                .ForMember(d => d.BetRequestId, opt => opt.MapFrom(src => src["BetRequestId"]))
                .ForMember(d => d.BetRequestTypeId, opt => opt.MapFrom(src => src["BetRequestTypeId"]))
                .ForMember(d => d.BetRequestTypeId, opt => opt.MapFrom(src => src["BetRequestTypeId"]))
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src["IDUtente"]))
                .ForMember(d => d.Session, opt => opt.MapFrom(src => src["Session"]))
                .ForMember(d => d.TicketId, opt => opt.MapFrom(src => src["TicketId"]))
                .ForMember(d => d.ExternalId, opt => opt.MapFrom(src => src["ExternalId"]))
                .ForMember(d => d.Game, opt => opt.MapFrom(src => src["Game"]))
                .ForMember(d => d.Games, opt => opt.MapFrom(src => src["Games"]))
                .ForMember(d => d.BetId, opt => opt.MapFrom(src => src["BetId"]))
                .ForMember(d => d.Ip, opt => opt.MapFrom(src => src["Ip"]))
                .ForMember(d => d.Timestamp, opt => opt.MapFrom(src => src["Timestamp"]));
            }).CreateMapper();

            var pars = new DynamicParameters(new
            {
                BetRequestId = betRequest.BetRequestId,
                BetRequestTypeId = betRequest.BetRequestTypeId,
                IdUtente = betRequest.UserId,
                Session = betRequest.Session,
                TicketId = betRequest.TicketId,
                ExternalId = betRequest.ExternalId,
                Game = betRequest.Game,
                Games = betRequest.Games,
                BetId = betRequest.BetId,
                Ip = betRequest.Ip,
                Timestamp = betRequest.Timestamp,
            });

            var res = _dataContext.ExecuteReaderProcedure<BetRequest>("dbo.SaveBetRequest", mapper, parameters: pars).FirstOrDefault();

            if (((res.BetRequestId ?? 0) > 0) && (betRequest.WebBetRequest != null))
            {
                res.WebBetRequest = betRequest.WebBetRequest;
                res.WebBetRequest.BetRequestId = res.BetRequestId;
                res.WebBetRequest = SaveWebBetRequest(betRequest.WebBetRequest);
            }

            if (((res.BetRequestId ?? 0) > 0) && (betRequest.ShopBetRequest != null))
            {
                res.ShopBetRequest = betRequest.ShopBetRequest;
                res.ShopBetRequest.BetRequestId = res.BetRequestId;
                res.ShopBetRequest = SaveShopBetRequest(betRequest.ShopBetRequest);
            }

            return res;

        }

        public WebBetRequest SaveWebBetRequest(WebBetRequest webBetrequest)
        {
            var pars = new DynamicParameters(new
            {
                BetRequestId = webBetrequest.BetRequestId,
                GroupCode = webBetrequest.GroupCode,
                ShopCode = webBetrequest.ShopCode,
                Skin = webBetrequest.Skin,
                UserAccount = webBetrequest.UserAccount,
                BetSettlementId = webBetrequest.BetSettlementId,
                BetSettlementReasonId = webBetrequest.BetSettlementReasonId
            });

            var res = _dataContext.ExecuteReaderProcedure<WebBetRequest>("dbo.SaveWebBetRequest", parameters: pars).FirstOrDefault();
            return res;
        }

        public ShopBetRequest SaveShopBetRequest(ShopBetRequest shopBetRequest)
        {

            var pars = new DynamicParameters(new
            {
                BetRequestId = shopBetRequest.BetRequestId,
                ShopId = shopBetRequest.ShopId,
                TerminalId = shopBetRequest.TerminalId,
                TerminalStr = shopBetRequest.TerminalStr,
                BetSettlementStatusId = shopBetRequest.BetSettlementStatusId,
                Loyalty = shopBetRequest.Loyalty,
                OperatorId = default(int?)
            });

            var res = _dataContext.ExecuteReaderProcedure<ShopBetRequest>("dbo.SaveShopBetRequest", parameters: pars).FirstOrDefault();
            return res;
        }

        public BetTransaction GetActiveTransaction(long betId, BetTransactionTypeEnum betTransactionTypeId, bool convertToRefund = false)
        {
            var mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<IDictionary<string, object>, BetTransaction>()
                .ForMember(d => d.BetTransactionId, opt => opt.MapFrom(src => src["BetTransactionId"]))
                .ForMember(d => d.BetTransactionTypeId, opt => opt.MapFrom(src => src["BetTransactionTypeId"]))
                .ForMember(d => d.BetRequestId, opt => opt.MapFrom(src => src["BetRequestId"]))
                .ForMember(d => d.BetId, opt => opt.MapFrom(src => src["BetId"]))
                .ForMember(d => d.Amount, opt => opt.MapFrom(src => src["Amount"]))
                .ForMember(d => d.CurrencyCode, opt => opt.MapFrom(src => src["CurrencyCode"]))
                .ForMember(d => d.TransactionId, opt => opt.MapFrom(src => src["IDTransazione"]))
                .ForMember(d => d.PaymentOrderId, opt => opt.MapFrom(src => src["IDCorrelazioneTransazioni"]))
                .ForMember(d => d.RefundBetTransactionId, opt => opt.MapFrom(src => src["RefundBetTransactionId"]));
            }).CreateMapper();

            var pars = new DynamicParameters(new
            {
                betId,
                betTransactionTypeId,
                convertToRefund
            });

            try
            {
                var res = _dataContext.ExecuteReaderProcedure<BetTransaction>("dbo.GetActiveTransaction", mapper, parameters: pars).FirstOrDefault();
                return res;
            }
            catch (Exception ex)
            {
                if ((ex is SqlException exp) && exp.Number == 60000)
                {
                    throw new IppicaException(ReturnCodeEnum.TransactionExistsButInIrregularState);
                }
                else
                    throw ex;
            };
        }

        public bool IsEmailSentToday(Email email)
        {
            var pars = new DynamicParameters(new
            {
                @From = email.From,
                @To = email.To,
                @Cc = email.Cc,
                @Subject = email.Subject,
                @BetRequestTypeId = email.BetRequestTypeId,
                @BetId = email.BetId,
                @TicketId = email.TicketId,
                @ExternalId = email.ExternalId,
            });

            var isSent = _dataContext.ExecuteReaderProcedure("[dbo].[IsEmailSentToday]", parameters: pars)
                                     .FirstOrDefault()?.Select(x => (bool)x.Value)?.FirstOrDefault();

            return isSent ?? false;
        }

        public void InsertLogEmail(Email email)
        {
            var pars = new DynamicParameters(new
            {
                email.BetRequestTypeId,
                email.BetId,
                email.TicketId,
                email.ExternalId,
                email.From,
                email.To,
                email.Cc,
                email.Subject,
                email.Message
            });

            var res = _dataContext.ExecuteProcedure("dbo.InsertLogEmail", parameters: pars);
        }
    }
}
