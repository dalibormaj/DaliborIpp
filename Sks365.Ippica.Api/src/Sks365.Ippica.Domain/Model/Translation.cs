using Sks365.Ippica.Common.Utility;
using System;

namespace Sks365.Ippica.Domain.Model
{
    public class Translation : BaseDomainModel
    {
        public string Code { get; set; }
        public string Text { get; set; }
        public int? TypeId { get; set; }
        public LanguageEnum? LanguageId { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}
