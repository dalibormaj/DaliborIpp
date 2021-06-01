using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.Domain.Utility
{
    public static class ApplicationTypeExtensions
    {
        public static TerminalTypeEnum? ConvertToTerminalTypeEnum(this ApplicationTypeEnum appTypeId)
        {
            TerminalTypeEnum? result = null;

            switch (appTypeId)
            {
                case ApplicationTypeEnum.Cdc:
                    result = TerminalTypeEnum.Standard;
                    break;
                case ApplicationTypeEnum.Prenotatore:
                    result = TerminalTypeEnum.Booking;
                    break;
                case ApplicationTypeEnum.Btt:
                    result = TerminalTypeEnum.SelfService;
                    break;
            }

            return result;
        }
    }
}
