using Sks365.Ippica.Common.Utility;
using Sks365.Ippica.Domain.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sks365.Ippica.Domain.Model
{
    public class User : BaseDomainModel
    {
        public int? UserId { get; set; }
        public BookmakerEnum? BookmakerId { get; set; }
        public int? ParentId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public UserStatusEnum? Status { get; set; }
        public byte? IsTestUser { get; set; }
        public UserTypeEnum? UserTypeId { get; set; }
        public SportWallet SportWallet { get; set; }
        public BonusWallet BonusWallet { get; set; }
        public string Description { get; set; }
        public string UserTypeDescription { get; set; }
        public CurrencyEnum? AccountCurrencyId { get; set; }
        public UserDetail Details { get; set; }
        public List<UserAdditionalData> AdditionalData { get; set; }
        public List<SelfLimitation> SelfLimitation { get; set; }

        public LanguageEnum GetUserLanguage()
        {
            var languageValue = AdditionalData?.FirstOrDefault(x => x?.UserDataTypeId == UserDataTypeEnum.Language)?.Value;
            if (!string.IsNullOrEmpty(languageValue))
            {
                var languageId = (LanguageEnum)Enum.Parse(typeof(LanguageEnum), languageValue);
                return languageId;
            }

            //Default User language
            var defaultLanguage = (BookmakerId != null && (BookmakerId == BookmakerEnum.IT || BookmakerId == BookmakerEnum.IT_SHOP)) ? LanguageEnum.Italian : LanguageEnum.English;
            return defaultLanguage;
        }

        public string GetUserLanguageCode()
        {
            return GetUserLanguage().GetDescription();
        }
    }
}
