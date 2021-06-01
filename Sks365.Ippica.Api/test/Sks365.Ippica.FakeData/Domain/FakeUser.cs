using Bogus;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.FakeData.Domain
{
    public class FakeUser : User, IFakeData<User>
    {
        private Faker<User> _fakeData;
        public FakeUser(int? userId)
        {
            _fakeData = new Faker<User>().RuleFor(x => x.UserId, x => userId ?? x.Random.Int(1000000, 5000000))
                                         .RuleFor(x => x.BookmakerId, x => BookmakerEnum.IT)
                                         .RuleFor(x => x.Name, x => x.Name.FindName())
                                         .RuleFor(x => x.Surname, x => x.Name.LastName())
                                         .RuleFor(x => x.Username, x => x.Internet.UserName())
                                         .RuleFor(x => x.Password, x => x.Internet.Password())
                                         .RuleFor(x => x.AdditionalData, x => new FakeUserAdditionalData().FakeData.Generate(3));
        }

        public FakeUser() : this(null)
        {
        }

        public Faker<User> FakeData => _fakeData;
    }
}
