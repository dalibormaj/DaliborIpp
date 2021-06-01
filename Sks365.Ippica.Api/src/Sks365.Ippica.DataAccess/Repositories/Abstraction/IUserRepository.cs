using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System.Collections.Generic;

namespace Sks365.Ippica.DataAccess.Repositories.Abstraction
{
    public interface IUserRepository : IRepository
    {
        List<SelfLimitation> CheckSelfLimitations(int userId, decimal amount, LanguageEnum languageId);
        SelfLimitation CheckSelfLimitation(int userId, decimal amount, LanguageEnum languageId, SelflimitationTypeEnum selflimitationType);
        List<User> GetUsers(int? userId = null, string userName = "", BookmakerEnum? bookmakerId = null);
        User GetUser(int userId);
        User GetUser(string userName, BookmakerEnum bookmakerId);
        UserDetail GetUserDetails(int userId);
        List<UserAdditionalData> GetUserAdditionalData(int userId);
        UserAdditionalData GetUserAdditionalDataValue(int userId, UserDataTypeEnum userDataType);
        string GetUserParameterValue(int userId, UserParameterTypeEnum? userParameterId);
        bool GetUserAvailibility(int userId, BookmakerEnum bookmakerId, ProviderEnum providerId, SectionTypeEnum sectionType);
    }
}
