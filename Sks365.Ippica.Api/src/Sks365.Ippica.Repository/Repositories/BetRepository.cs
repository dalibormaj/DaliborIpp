using Dapper;
using Sks365Ippica.Common.Exceptions;
using Sks365Ippica.Common.Utility;
using Sks365Ippica.Domain.Model;
using Sks365Ippica.Domain.Model.Enums;
using Sks365Ippica.Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Sks365Ippica.Repository.Repositories
{
    internal class BetRepository : IBetRepository
    {
        private readonly IDataContext _dataContext;

        public BetRepository(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public List<BetRequest> GetBetRequests(long? betRequestId = null, long? betId = null, BetRequestTypeEnum? betRequestTypeId = null)
        {
            var pars = new DynamicParameters(new
            {
                BetRequestId = betRequestId,
                BetId = betId,
                BetRequestTypeId = betRequestTypeId
            });

            var res = _dataContext.ExecuteReaderProcedure<BetRequest>("dbo.GetBetRequest", parameters: pars);

            //TODO!!! Please optimize this!!!
            if ((res?.Count ?? 0) > 0)
            {
                foreach (var item in res)
                {
                    item.WebBetRequest = GetWebBetRequest((long)item.BetRequestId);
                    item.ShopBetRequest = GetShopBetRequest((long)item.BetRequestId);
                }
            }

            return res;
        }

        public BetRequest GetBetRequest(long betRequestId)
        {
            var betRequests = GetBetRequests(betRequestId: betRequestId);
            if ((betRequests?.Count ?? 0) != 1)
            {
                throw new BadRequestException(ReturnCodeEnum.BetNotFound, $"Unable to find the bet request with the BetRequestId: {betRequestId}");
            }

            return betRequests?.FirstOrDefault();
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


        public List<Bet> GetBets(int? userId = null, long? betId = null, string ticketId = "")
        {
            var pars = new DynamicParameters(new
            {
                IDUtente = userId,
                BetId = betId,
                TicketId = ticketId
            });

            var res = _dataContext.ExecuteReaderProcedure("dbo.GetBets", parameters: pars);

            var map = res.GroupBy(x => new
            {
                BetId = (long)x["BetId"],
                BetStatusId = x["BetStatusId"] == null ? null : (BetStatusEnum?)(int?)x["BetStatusId"],
                BetTypeId = x["BetTypeId"] == null ? null : (BetTypeEnum?)(int?)x["BetTypeId"],
                IDUtente = x["IDUtente"] == null ? null : (int?)x["IDUtente"],
                TicketId = x["TicketId"] == null ? null : x["TicketId"].ToString(),
                Amount = x["Amount"] == null ? null : (decimal?)x["Amount"],
                TotalAmount = x["TotalAmount"] == null ? null : (decimal?)x["TotalAmount"],
                RefundAmount = x["RefundAmount"] == null ? null : (decimal?)x["RefundAmount"],
                CurrencyCode = x["CurrencyCode"] == null ? null : (CurrencyEnum?)Enum.Parse(typeof(CurrencyEnum), x["CurrencyCode"].ToString()),
                TaxStake = x["TaxStake"] == null ? null : (decimal?)x["TaxStake"],
                TaxWin = x["TaxWin"] == null ? null : (decimal?)x["TaxWin"],
                ExternalId = x["ExternalId"] == null ? null : x["ExternalId"].ToString(),
                Emission = x["Emission"] == null ? null : (DateTime?)x["Emission"],
                Stake = x["Stake"] == null ? null : (decimal?)x["Stake"],
                MaxWinning = x["MaxWinning"] == null ? null : (decimal?)x["MaxWinning"],
                Bets = x["Bets"] == null ? null : (int?)x["Bets"],
                Competence = x["Competence"] == null ? null : (DateTime?)x["Competence"],
                Type = x["Type"] == null ? null : (int?)x["Type"],
                Bonus = x["Bonus"] == null ? null : (decimal?)x["Bonus"],
                Source = x["Source"] == null ? null : (int?)x["Source"]
            }).Select(g => new Bet()
            {
                BetId = g.Key.BetId,
                BetStatusId = g.Key.BetStatusId,
                BetTypeId = g.Key.BetTypeId,
                IdUtente = g.Key.IDUtente,
                TicketId = g.Key.TicketId,
                Amount = g.Key.Amount,
                TotalAmount = g.Key.TotalAmount,
                RefundAmount = g.Key.RefundAmount,
                CurrencyCode = g.Key.CurrencyCode,
                TaxStake = g.Key.TaxStake,
                TaxWin = g.Key.TaxWin,
                ExternalId = g.Key.ExternalId,
                Emission = g.Key.Emission,
                Stake = g.Key.Stake,
                MaxWinning = g.Key.MaxWinning,
                Bets = g.Key.Bets,
                Competence = g.Key.Competence,
                Type = g.Key.Type,
                Bonus = g.Key.Bonus,
                Source = g.Key.Source,
                FixBetDetails = g.Select(b => new FixBetDetail()
                {
                    FixBetDetailsId = b["FixBetDetailsId"] == null ? null : (long?)b["FixBetDetailsId"],
                    Race = b["Race"] == null ? null : (int?)b["Race"],
                    Palinsesto = b["Palinsesto"] == null ? null : (int?)b["Palinsesto"],
                    Avvenimento = b["Avvenimento"] == null ? null : (int?)b["Avvenimento"],
                    Course = b["Course"] == null ? null : b["Course"].ToString(),
                    Number = b["Number"] == null ? null : (int?)b["Number"],
                    RDate = b["RDate"] == null ? null : (DateTime?)b["RDate"],
                    Code = b["Code"] == null ? null : (int?)b["Code"],
                    Market = b["Market"] == null ? null : b["Market"].ToString(),
                    InfoAggDes = b["InfoAggDes"] == null ? null : b["InfoAggDes"].ToString(),
                    Horse = b["Horse"] == null ? null : (int?)b["Horse"],
                    Odd = b["Odd"] == null ? null : (decimal?)b["Odd"],
                    //System = b["System"] == null ? null : b["System"].ToString(),
                    InfoAgg = b["InfoAgg"] == null ? null : b["InfoAgg"].ToString(),
                    //NumCombs = b["NumCombs"] == null ? null : (int?)b["NumCombs"]
                }).ToList()
            }).ToList();

            return map;
        }

        public Bet GetBet(string ticketId)
        {
            var bets = GetBets(ticketId: ticketId);
            if (bets == null || (bets != null && bets.Count != 1))
            {
                throw new BadRequestException(ReturnCodeEnum.BetNotFound, $"Unable to find the bet - ticketId: {ticketId}");
            }

            return bets?.FirstOrDefault();
        }

        public Bet SaveBet(Bet bet)
        {
            var pars = new DynamicParameters(new
            {
                BetId = bet.BetId,
                BetStatusId = bet.BetStatusId,
                BetTypeId = bet.BetTypeId,
                IdUtente = bet.IdUtente,
                TicketId = bet.TicketId,
                Amount = bet.Amount,
                TotalAmount = bet.TotalAmount,
                PaymentAmount = bet.PaymentAmount,
                RefundAmount = bet.RefundAmount,
                CurrencyCode = Enum.GetName(typeof(CurrencyEnum), bet.CurrencyCode),
                TaxStake = bet.TaxStake,
                TaxWin = bet.TaxWin,
                ExternalId = bet.ExternalId,
                Emission = bet.Emission,
                Stake = bet.Stake,
                MaxWinning = bet.MaxWinning,
                Bets = bet.Bets,
                Competence = bet.Competence,
                Type = bet.Type,
                BonusId = bet.BonusId,
                Bonus = bet.Bonus,
                Source = bet.Source
            });

            var res = _dataContext.ExecuteReaderProcedure<Bet>("dbo.SaveBet", parameters: pars).FirstOrDefault();

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

        public List<FixBetDetail> SaveFixBetDetails(List<FixBetDetail> betDetails)
        {
            DataTable dtBetDetails = new DataTable("FixBetDetails");
            dtBetDetails.Columns.Add("FixBetDetailsId", typeof(long));
            dtBetDetails.Columns.Add("BetId", typeof(long));
            dtBetDetails.Columns.Add("Race", typeof(int));
            dtBetDetails.Columns.Add("Palinsesto", typeof(int));
            dtBetDetails.Columns.Add("Avvenimento", typeof(int));
            dtBetDetails.Columns.Add("Course");
            dtBetDetails.Columns.Add("Number", typeof(int));
            dtBetDetails.Columns.Add("RDate", typeof(DateTime));
            dtBetDetails.Columns.Add("Code", typeof(int));
            dtBetDetails.Columns.Add("Market");
            dtBetDetails.Columns.Add("InfoAggDes");
            dtBetDetails.Columns.Add("Horse", typeof(int));
            dtBetDetails.Columns.Add("Odd", typeof(decimal));
            dtBetDetails.Columns.Add("InfoAgg");

            foreach (FixBetDetail bd in betDetails)
            {
                DataRow drBetDetail = dtBetDetails.NewRow();
                drBetDetail["FixBetDetailsId"] = bd.FixBetDetailsId.HasValue ? bd.FixBetDetailsId : (object)DBNull.Value;
                drBetDetail["BetId"] = bd.BetId.HasValue ? bd.BetId : (object)DBNull.Value;
                drBetDetail["Race"] = bd.Race.HasValue ? bd.Race : (object)DBNull.Value;
                drBetDetail["Palinsesto"] = bd.Palinsesto.HasValue ? bd.Palinsesto : (object)DBNull.Value;
                drBetDetail["Avvenimento"] = bd.Avvenimento.HasValue ? bd.Avvenimento : (object)DBNull.Value;
                drBetDetail["Course"] = bd.Course;
                drBetDetail["Number"] = bd.Number.HasValue ? bd.Number : (object)DBNull.Value;
                drBetDetail["RDate"] = bd.RDate.HasValue ? bd.RDate : (object)DBNull.Value;
                drBetDetail["Code"] = bd.Code.HasValue ? bd.Code : (object)DBNull.Value;
                drBetDetail["Market"] = bd.Market;
                drBetDetail["InfoAggDes"] = bd.InfoAggDes;
                drBetDetail["Horse"] = bd.Horse.HasValue ? bd.Horse : (object)DBNull.Value;
                drBetDetail["Odd"] = bd.Odd.HasValue ? bd.Odd : (object)DBNull.Value;
                //drBetDetail["System"] = bd.System;
                drBetDetail["InfoAgg"] = bd.InfoAgg;
                //drBetDetail["NumCombs"] = bd.NumCombs.HasValue ? bd.NumCombs : (object)DBNull.Value;

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

            if ((res.PsrBetDetailsId ?? 0) > 0 && psrBetDetail.Scommessa != null)
            {
                psrBetDetail.Scommessa.PsrBetDetailsId = res.PsrBetDetailsId;
                psrBetDetail.Scommessa.BetId = res.BetId;
                res.Scommessa = SavePsrScommessa(psrBetDetail.Scommessa);
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
                Moltiplicatore = psrScommessa.Moltiplicatore,
                GruppoCodice = psrScommessa.GruppoCodice,
                EventoCodice = psrScommessa.EventoCodice,
                EventoScom = psrScommessa.EventoScom,
                EventoSistema = psrScommessa.EventoSistema,
                EventoMappa = psrScommessa.EventoMappa
            });

            var res = _dataContext.ExecuteReaderProcedure<PsrScommessa>("dbo.SavePsrScommessa", parameters: pars).FirstOrDefault();

            return res;
        }

        public BetRequest SaveBetRequest(BetRequest betRequest)
        {
            var pars = new DynamicParameters(new
            {
                BetRequestId = betRequest.BetRequestId,
                BetRequestTypeId = betRequest.BetRequestTypeId,
                IdUtente = betRequest.IdUtente,
                Session = betRequest.Session,
                TicketId = betRequest.TicketId,
                Game = betRequest.Game,
                Games = betRequest.Games,
                BetId = betRequest.BetId,
                Ip = betRequest.Ip,
                Timestamp = betRequest.Timestamp,
                IdTransazione = betRequest.IdTransazione,
                IdCorrelazioneTransazioni = betRequest.IdCorrelazioneTransazioni
            });

            var res = _dataContext.ExecuteReaderProcedure<BetRequest>("dbo.SaveBetRequest", parameters: pars).FirstOrDefault();

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
                BetSettlementStatusId = shopBetRequest.BetSettlementStatusId
            });

            var res = _dataContext.ExecuteReaderProcedure<ShopBetRequest>("dbo.SaveShopBetRequest", parameters: pars).FirstOrDefault();

            return res;
        }
    }
}
