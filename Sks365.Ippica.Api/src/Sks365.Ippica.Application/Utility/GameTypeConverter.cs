using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sks365.Ippica.Application.Utility
{
    public static class GameTypeConverter
    {
        public static BetTypeEnum? GameToBetTypeEnum(string game)
        {
            switch (game)
            {
                case var type when string.Equals(type, "QF", StringComparison.InvariantCultureIgnoreCase):
                    return BetTypeEnum.Fix;
                case var type when string.Equals(type, "TOT", StringComparison.InvariantCultureIgnoreCase):
                    return BetTypeEnum.PsipTote;
                case var type when string.Equals(type, "PSR", StringComparison.InvariantCultureIgnoreCase):
                    return BetTypeEnum.PsrTote;
                default:
                    return null;
            }
        }

        public static string BetTypeEnumToGameString(BetTypeEnum? betTypeEnum)
        {
            switch (betTypeEnum)
            {
                case BetTypeEnum.Fix:
                    return "QF";
                case BetTypeEnum.PsipTote:
                    return "TOT";
                case BetTypeEnum.PsrTote:
                    return "PSR";
                default:
                    return null;
            }
        }
    }
}
