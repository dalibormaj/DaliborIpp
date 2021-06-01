using Bogus;
using Sks365.Ippica.Domain.Model;
using Sks365.Ippica.Domain.Model.Enums;

namespace Sks365.Ippica.FakeData.Domain
{
    public class FakeUserAdditionalData : UserAdditionalData, IFakeData<UserAdditionalData>
    {
        private Faker<UserAdditionalData> _fakeData;
        private string[] usualAdditionalValues = { null, "0", "1" };

        public FakeUserAdditionalData()
        {
            _fakeData = new Faker<UserAdditionalData>().RuleFor(x => x.UserDataTypeId, x => x.PickRandom<UserDataTypeEnum>())
                                                       .RuleFor(x => x.Value, x => x.PickRandom(usualAdditionalValues));
        }
        public Faker<UserAdditionalData> FakeData => _fakeData;
    }
}
