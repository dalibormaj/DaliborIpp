using AutoMapper;
using System.Collections.Generic;

namespace Sks365.Ippica.Common.Utility
{
    public class MapperLocator : IMapperLocator
    {
        private Dictionary<MapperName, IMapper> _mappers;

        public MapperLocator(Dictionary<MapperName, IMapper> mappers)
        {
            _mappers = mappers;
        }

        public IMapper GetMapper(MapperName mapperName)
        {
            return _mappers[mapperName];
        }
    }

}
