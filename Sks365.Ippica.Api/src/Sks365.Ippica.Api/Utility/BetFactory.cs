using AutoMapper;
using Sks365.Ippica.Api.Dto;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;

namespace Sks365.Ippica.Api.Utility
{
    public class BetFactory
    {
        #region Web
        public static Bet Create(WebReserveBetRequest<BetDto> dto, IMapper mapper)
        {

            List<FixBetDetail> fixBetDetails = mapper.Map<List<FixBetDetail>>(dto.Bet.Details);
            List<SysBetDetail> sysBetDetails = mapper.Map<List<SysBetDetail>>(dto.Bet.Details);

            Bet bet;

            if ((fixBetDetails?.Count ?? 0) > 0)
            {
                bet = new Bet(fixBetDetails);
            }
            else
            {
                bet = new Bet(sysBetDetails);
            }

            bet.BetStatusId = BetStatusEnum.Reserved;
            bet.TicketId = dto.Bet.Header.TicketId;
            bet.Amount = (decimal)dto.Amount / 100;
            bet.Stake = (decimal)dto.Bet.Header.Stake / 100;
            bet.CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), dto.Currency);
            bet.TaxStake = (decimal)dto.TaxStake / 100;
            bet.TaxWin = (decimal)dto.TaxWin / 100;
            bet.Emission = dto.Bet.Header.Emission;
            bet.EmissionUtc = dto.Bet.Header.EmissionUtc;
            bet.MaxWinning = (decimal)dto.Bet.Header.MaxWinning / 100;
            bet.Bets = dto.Bet.Header.Bets;
            bet.Competence = dto.Bet.Header.Competence;
            bet.Type = dto.Bet.Header.Type;
            bet.BonusId = dto.BonusId;
            bet.Bonus = (decimal)dto.Bonus / 100;
            bet.Source = dto.Bet.Header.Source;
            bet.Antepost = dto.Bet.Header.Antepost;

            return bet;
        }

        public static Bet Create(WebReserveBetRequest<PsipBetDto> dto, IMapper mapper)
        {
            PsipBetDetail psipBetDetails = mapper.Map<PsipBetDetail>(dto.Bet);

            var result = new Bet(psipBetDetails)
            {
                BetStatusId = BetStatusEnum.Reserved,
                TicketId = dto.TicketId,
                Amount = (decimal)dto.Amount / 100,
                Stake = (decimal)dto.Bet.Prezzo / 100,
                CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), dto.Currency),
                TaxStake = (decimal)dto.TaxStake / 100,
                TaxWin = (decimal)dto.TaxWin / 100,
                Emission = dto.Bet.Emission,
                EmissionUtc = dto.Bet.EmissionUtc,
                Competence = dto.Bet.Competenza,
                BonusId = dto.BonusId,
                Bonus = (decimal)dto.Bonus / 100,
                Source = dto.Bet.TipoGiocata
            };

            return result;
        }

        public static Bet Create(WebReserveBetRequest<PsrBetDto> dto, IMapper mapper)
        {
            PsrBetDetail psrBetDetails = mapper.Map<PsrBetDetail>(dto.Bet);

            var result = new Bet(psrBetDetails)
            {
                BetStatusId = BetStatusEnum.Reserved,
                TicketId = dto.TicketId,
                Amount = (decimal)dto.Amount / 100,
                Stake = (decimal)dto.Bet.Prezzo / 100,
                CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), dto.Currency),
                TaxStake = (decimal)dto.TaxStake / 100,
                TaxWin = (decimal)dto.TaxWin / 100,
                Emission = dto.Bet.Emission,
                EmissionUtc = dto.Bet.EmissionUtc,
                Competence = dto.Bet.Competenza,
                BonusId = dto.BonusId,
                Bonus = (decimal)dto.Bonus / 100,
                Source = dto.Bet.TipoGiocata
            };

            return result;
        }

        public static Bet Create(WebPlaceBetRequest<BetDto> dto, IMapper mapper)
        {

            List<FixBetDetail> fixBetDetails = mapper.Map<List<FixBetDetail>>(dto.Bet.Details);
            List<SysBetDetail> sysBetDetails = mapper.Map<List<SysBetDetail>>(dto.Bet.Details);

            Bet bet;

            if ((fixBetDetails?.Count ?? 0) > 0)
            {
                bet = new Bet(fixBetDetails);
            }
            else
            {
                bet = new Bet(sysBetDetails);
            }

            bet.BetStatusId = BetStatusEnum.Placed;
            bet.TicketId = dto.Bet.Header.TicketId;
            bet.Amount = (decimal)dto.Amount / 100;
            bet.Stake = (decimal)dto.Bet.Header.Stake / 100;
            bet.CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), dto.Currency);
            bet.TaxStake = (decimal)dto.TaxStake / 100;
            bet.TaxWin = (decimal)dto.TaxWin / 100;
            bet.ExternalId = dto.ExternalId;
            bet.Emission = dto.Bet.Header.Emission;
            bet.EmissionUtc = dto.Bet.Header.EmissionUtc;
            bet.MaxWinning = (decimal)dto.Bet.Header.MaxWinning / 100;
            bet.Bets = dto.Bet.Header.Bets;
            bet.Competence = dto.Bet.Header.Competence;
            bet.Type = dto.Bet.Header.Type;
            bet.BonusId = dto.BonusId;
            bet.Bonus = (decimal)dto.Bonus / 100;
            bet.Source = dto.Bet.Header.Source;
            bet.Antepost = dto.Bet.Header.Antepost;

            return bet;
        }


        public static Bet Create(WebPlaceBetRequest<PsipBetDto> dto, IMapper mapper)
        {
            PsipBetDetail psipBetDetails = mapper.Map<PsipBetDetail>(dto.Bet);

            var result = new Bet(psipBetDetails)
            {
                BetStatusId = BetStatusEnum.Placed,
                TicketId = dto.TicketId,
                Amount = (decimal)dto.Amount / 100,
                Stake = (decimal)dto.Bet.Prezzo / 100,
                CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), dto.Currency),
                TaxStake = (decimal)dto.TaxStake / 100,
                TaxWin = (decimal)dto.TaxWin / 100,
                ExternalId = dto.ExternalId,
                Emission = dto.Bet.Emission,
                EmissionUtc = dto.Bet.EmissionUtc,
                Competence = dto.Bet.Competenza,
                BonusId = dto.BonusId,
                Bonus = (decimal)dto.Bonus / 100,
            };

            return result;
        }

        public static Bet Create(WebPlaceBetRequest<PsrBetDto> dto, IMapper mapper)
        {
            PsrBetDetail psrBetDetails = mapper.Map<PsrBetDetail>(dto.Bet);

            var result = new Bet(psrBetDetails)
            {
                BetStatusId = BetStatusEnum.Placed,
                TicketId = dto.TicketId,
                Amount = (decimal)dto.Amount / 100,
                Stake = (decimal)dto.Bet.Prezzo / 100,
                CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), dto.Currency),
                TaxStake = (decimal)dto.TaxStake / 100,
                TaxWin = (decimal)dto.TaxWin / 100,
                ExternalId = dto.ExternalId,
                Emission = dto.Bet.Emission,
                EmissionUtc = dto.Bet.EmissionUtc,
                Competence = dto.Bet.Competenza,
                BonusId = dto.BonusId,
                Bonus = (decimal)dto.Bonus / 100
            };

            return result;
        }
        #endregion

        #region Shop
        public static Bet Create(ShopReserveBetRequest<BetDto> dto, IMapper mapper)
        {

            List<FixBetDetail> fixBetDetails = mapper.Map<List<FixBetDetail>>(dto.JBet.Details);
            List<SysBetDetail> sysBetDetails = mapper.Map<List<SysBetDetail>>(dto.JBet.Details);

            Bet bet;

            if ((fixBetDetails?.Count ?? 0) > 0)
            {
                bet = new Bet(fixBetDetails);
            }
            else
            {
                bet = new Bet(sysBetDetails);
            }

            bet.BetStatusId = BetStatusEnum.Reserved;
            bet.TicketId = dto.JBet.Header.TicketId;
            bet.Amount = (decimal)dto.Amount / 100;
            bet.Stake = (decimal)dto.JBet.Header.Stake / 100;
            bet.CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), dto.Currency);
            bet.TaxStake = (decimal)dto.TaxStake / 100;
            bet.TaxWin = (decimal)dto.TaxWin / 100;
            bet.Emission = dto.JBet.Header.Emission;
            bet.EmissionUtc = dto.JBet.Header.EmissionUtc;
            bet.MaxWinning = (decimal)dto.JBet.Header.MaxWinning / 100;
            bet.Bets = dto.JBet.Header.Bets;
            bet.Competence = dto.JBet.Header.Competence;
            bet.Type = dto.JBet.Header.Type;
            bet.Source = dto.JBet.Header.Source;
            bet.Antepost = dto.JBet.Header.Antepost;

            return bet;
        }

        public static Bet Create(ShopReserveBetRequest<PsipBetDto> dto, IMapper mapper)
        {
            PsipBetDetail psipBetDetails = mapper.Map<PsipBetDetail>(dto.JBet);

            var result = new Bet(psipBetDetails)
            {
                BetStatusId = BetStatusEnum.Reserved,
                TicketId = dto.TicketId,
                Amount = (decimal)dto.Amount / 100,
                Stake = (decimal)dto.JBet.Prezzo / 100,
                CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), dto.Currency),
                TaxStake = (decimal)dto.TaxStake / 100,
                TaxWin = (decimal)dto.TaxWin / 100,
                Emission = dto.JBet.Emission,
                EmissionUtc = dto.JBet.EmissionUtc,
                Competence = dto.JBet.Competenza
            };

            return result;
        }

        public static Bet Create(ShopReserveBetRequest<PsrBetDto> dto, IMapper mapper)
        {
            PsrBetDetail psrBetDetails = mapper.Map<PsrBetDetail>(dto.JBet);

            var result = new Bet(psrBetDetails)
            {
                BetStatusId = BetStatusEnum.Reserved,
                TicketId = dto.TicketId,
                Amount = (decimal)dto.Amount / 100,
                Stake = (decimal)dto.JBet.Prezzo / 100,
                CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), dto.Currency),
                TaxStake = (decimal)dto.TaxStake / 100,
                TaxWin = (decimal)dto.TaxWin / 100,
                Emission = dto.JBet.Emission,
                EmissionUtc = dto.JBet.EmissionUtc,
                Competence = dto.JBet.Competenza
            };

            return result;
        }

        public static Bet Create(ShopPlaceBetRequest<BetDto> dto, IMapper mapper)
        {

            List<FixBetDetail> fixBetDetails = mapper.Map<List<FixBetDetail>>(dto.JBet.Details);
            List<SysBetDetail> sysBetDetails = mapper.Map<List<SysBetDetail>>(dto.JBet.Details);

            Bet bet;

            if ((fixBetDetails?.Count ?? 0) > 0)
            {
                bet = new Bet(fixBetDetails);
            }
            else
            {
                bet = new Bet(sysBetDetails);
            }

            bet.BetStatusId = BetStatusEnum.Placed;
            bet.TicketId = dto.JBet.Header.TicketId;
            bet.Amount = (decimal)dto.Amount / 100;
            bet.Stake = (decimal)dto.JBet.Header.Stake / 100;
            bet.CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), dto.Currency);
            bet.TaxStake = (decimal)dto.TaxStake / 100;
            bet.TaxWin = (decimal)dto.TaxWin / 100;
            bet.ExternalId = dto.ExternalId;
            bet.Emission = dto.JBet.Header.Emission;
            bet.EmissionUtc = dto.JBet.Header.EmissionUtc;
            bet.MaxWinning = (decimal)dto.JBet.Header.MaxWinning / 100;
            bet.Bets = dto.JBet.Header.Bets;
            bet.Competence = dto.JBet.Header.Competence;
            bet.Type = dto.JBet.Header.Type;
            bet.Source = dto.JBet.Header.Source;
            bet.Antepost = dto.JBet.Header.Antepost;

            return bet;
        }


        public static Bet Create(ShopPlaceBetRequest<PsipBetDto> dto, IMapper mapper)
        {
            PsipBetDetail psipBetDetails = mapper.Map<PsipBetDetail>(dto.JBet);

            var result = new Bet(psipBetDetails)
            {
                BetStatusId = BetStatusEnum.Placed,
                TicketId = dto.TicketId,
                Amount = (decimal)dto.Amount / 100,
                Stake = (decimal)dto.JBet.Prezzo / 100,
                CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), dto.Currency),
                TaxStake = (decimal)dto.TaxStake / 100,
                TaxWin = (decimal)dto.TaxWin / 100,
                ExternalId = dto.ExternalId,
                Emission = dto.JBet.Emission,
                EmissionUtc = dto.JBet.EmissionUtc,
                Competence = dto.JBet.Competenza
            };

            return result;
        }

        public static Bet Create(ShopPlaceBetRequest<PsrBetDto> dto, IMapper mapper)
        {
            PsrBetDetail psrBetDetails = mapper.Map<PsrBetDetail>(dto.JBet);

            var result = new Bet(psrBetDetails)
            {
                BetStatusId = BetStatusEnum.Placed,
                TicketId = dto.TicketId,
                Amount = (decimal)dto.Amount / 100,
                Stake = (decimal)dto.JBet.Prezzo / 100,
                CurrencyId = (CurrencyEnum)Enum.Parse(typeof(CurrencyEnum), dto.Currency),
                TaxStake = (decimal)dto.TaxStake / 100,
                TaxWin = (decimal)dto.TaxWin / 100,
                ExternalId = dto.ExternalId,
                Emission = dto.JBet.Emission,
                EmissionUtc = dto.JBet.EmissionUtc,
                Competence = dto.JBet.Competenza
            };

            return result;
        }
        #endregion
    }
}
