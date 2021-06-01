using System;

namespace Sks365.Ippica.Domain.Model
{
    public class UserDetail : BaseDomainModel
    {
        public int? UserId { get; set; }
        public string Note { get; set; }
        public string ReservedNote { get; set; }
        public int? Color { get; set; }
        public DateTime? LastLogin { get; set; }
        public int? CountryId { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Province { get; set; }
        public string Region { get; set; }
        public string Telephone { get; set; }
        public string MobilePhone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? SecurityQuestionId { get; set; }
        public string SecurityAnswer { get; set; }
        public int? IDGMT { get; set; }
        public string InternationalCountryCodeId { get; set; }
        public int? BetsReserveUserTypeId { get; set; }
    }
}
