using AutoMapper;


namespace Sks365.Ippica.Common.Utility
{
    public interface IMapperLocator
    {
        IMapper GetMapper(MapperName mapperName);
    }
}
