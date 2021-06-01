using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System.Collections.Generic;

namespace Sks365.Ippica.Application.Services.Abstraction
{
    public interface IUserService
    {
        User GetUser(string userName, BookmakerEnum bookmakerId, bool getWallets = false, bool getUserDetails = false, bool getAdditionalData = false);
        User GetUser(string session, bool getWallets = false, bool getUserDetails = false, bool getAdditionalData = false);
        User GetUser(int userId, bool getWallets = false, bool getUserDetails = false, bool getAdditionalData = false);
        User GetUserByTicketId(string ticketId, bool getWallets = false, bool getUserDetails = false, bool getAdditionalData = false);
        SportWallet GetSportWallet(int userId);
        BonusWallet GetBonusWallet(int userId);
        List<UserAdditionalData> GetUserAdditionalData(int userId);
        UserAdditionalData GetUserAdditionalDataValue(int userId, UserDataTypeEnum dataType);
        string GetUserParameterValue(int userId, UserParameterTypeEnum? userParameterId);
    }
}
