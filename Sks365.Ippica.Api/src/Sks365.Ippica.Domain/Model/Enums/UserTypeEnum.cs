namespace Sks365.Ippica.Domain.Model.Enums
{
    public enum UserTypeEnum : byte
    {
        User = 0,
        AnonymousUser = 5,
        Agent = 20,
        Administrator = 40,
        CommissionsTemplate = 100,
        Company = 254,
        IsbetsAdmin = 255
    }
}