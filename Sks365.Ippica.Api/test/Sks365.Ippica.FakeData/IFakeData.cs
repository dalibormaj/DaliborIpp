using Bogus;

namespace Sks365.Ippica.FakeData
{
    public interface IFakeData<T> where T : class
    {
        Faker<T> FakeData { get; }
    }
}
