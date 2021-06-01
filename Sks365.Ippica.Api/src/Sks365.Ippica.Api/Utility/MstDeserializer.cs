using Newtonsoft.Json;
using Sks365.Ippica.Api.Dto;
using Sks365.Ippica.Api.Dto.Requests;
using Sks365.Ippica.Common.Exceptions;
using Sks365.Ippica.Common.Utility;
using System;
using System.Collections.Generic;

namespace Sks365.Ippica.Api.Utility
{
    public static class MstDeserializer
    {
        /// <summary>
        /// Try to deserialize the raw json into one of the types given by the parameter "types"
        /// In case of failure the Exception will be thrown.
        /// </summary>
        /// <param name="json">Raw json to be converted</param>
        /// <param name="types">Type candidates. Try to convert into one of these types. Note: First success conversion will be returned</param>
        /// <returns></returns>
        public static object Deserialize(string json, List<Type> types)
        {
            object result = null;

            foreach (var type in types)
            {
                try
                {
                    result = JsonConvert.DeserializeObject(json, type, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error });
                    break;
                }
                catch (Exception ex)
                {
                };
            }

            if (result == null)
            {
                throw new IppicaException(ReturnCodeEnum.BadRequest, "Bad request");
            }

            return result;
        }

        public static dynamic DeserializeWebReserve(string json)
        {
            var fixBet = TryDeserialize<WebReserveBetRequest<BetDto>>(json);
            var psrBet = TryDeserialize<WebReserveBetRequest<PsrBetDto>>(json);
            var psipBet = TryDeserialize<WebReserveBetRequest<PsipBetDto>>(json);

            if (!string.IsNullOrEmpty(fixBet?.Game) && fixBet.Game.Equals("QF", StringComparison.OrdinalIgnoreCase))
                return fixBet;
            else if (!string.IsNullOrEmpty(psrBet?.Game) && psrBet.Game.Equals("PSR", StringComparison.OrdinalIgnoreCase))
                return psrBet;
            else if (!string.IsNullOrEmpty(psipBet?.Game) && psipBet.Game.Equals("TOT", StringComparison.OrdinalIgnoreCase))
                return psipBet;

            return null;
        }

        public static dynamic DeserializeWebPlace(string json)
        {

            var fixBet = TryDeserialize<WebPlaceBetRequest<BetDto>>(json);
            var psrBet = TryDeserialize<WebPlaceBetRequest<PsrBetDto>>(json);
            var psipBet = TryDeserialize<WebPlaceBetRequest<PsipBetDto>>(json);

            if (!string.IsNullOrEmpty(fixBet?.Game) && fixBet.Game.Equals("QF", StringComparison.OrdinalIgnoreCase))
                return fixBet;
            else if (!string.IsNullOrEmpty(psrBet?.Game) && psrBet.Game.Equals("PSR", StringComparison.OrdinalIgnoreCase))
                return psrBet;
            else if (!string.IsNullOrEmpty(psipBet?.Game) && psipBet.Game.Equals("TOT", StringComparison.OrdinalIgnoreCase))
                return psipBet;

            return null;
        }

        public static dynamic DeserializeShopReserve(string json)
        {
            var fixBet = TryDeserialize<ShopReserveBetRequest<BetDto>>(json);
            var psrBet = TryDeserialize<ShopReserveBetRequest<PsrBetDto>>(json);
            var psipBet = TryDeserialize<ShopReserveBetRequest<PsipBetDto>>(json);

            if (!string.IsNullOrEmpty(fixBet?.Game) && fixBet.Game.Equals("QF", StringComparison.OrdinalIgnoreCase))
                return fixBet;
            else if (!string.IsNullOrEmpty(psrBet?.Game) && psrBet.Game.Equals("PSR", StringComparison.OrdinalIgnoreCase))
                return psrBet;
            else if (!string.IsNullOrEmpty(psipBet?.Game) && psipBet.Game.Equals("TOT", StringComparison.OrdinalIgnoreCase))
                return psipBet;

            return null;
        }

        public static dynamic DeserializeShopPlace(string json)
        {

            var fixBet = TryDeserialize<ShopPlaceBetRequest<BetDto>>(json);
            var psrBet = TryDeserialize<ShopPlaceBetRequest<PsrBetDto>>(json);
            var psipBet = TryDeserialize<ShopPlaceBetRequest<PsipBetDto>>(json);

            if (!string.IsNullOrEmpty(fixBet?.Game) && fixBet.Game.Equals("QF", StringComparison.OrdinalIgnoreCase))
                return fixBet;
            else if (!string.IsNullOrEmpty(psrBet?.Game) && psrBet.Game.Equals("PSR", StringComparison.OrdinalIgnoreCase))
                return psrBet;
            else if (!string.IsNullOrEmpty(psipBet?.Game) && psipBet.Game.Equals("TOT", StringComparison.OrdinalIgnoreCase))
                return psipBet;

            return null;
        }

        private static T TryDeserialize<T>(string json)
        {
            T result = default(T);

            try
            {
                result = JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
            };

            return result;
        }
    }
}
