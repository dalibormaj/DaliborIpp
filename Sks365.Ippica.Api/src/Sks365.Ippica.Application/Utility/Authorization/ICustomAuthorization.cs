using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;
using System.Collections.Generic;

namespace Sks365.Ippica.Application.Utility.Authorization
{
    public interface ICustomAuthorization
    {
        ICustomAuthorization ApplyConditions(List<CustomAuthorizationConditions> conditions);
        void Autorize(User user);
        void Autorize(int userId);
        void Autorize(string userName, BookmakerEnum bookmakerId);
    }
}
