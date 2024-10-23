using AutoMapper;
using GeoJsonRandom.Core.Models;
using GeoJsonRandom.Models;

namespace GeoJsonRandom.Web.Mappers
{
    public class GeoDataProfile : Profile
    {
        public GeoDataProfile()
        {
            CreateMap<GeoDataResultDto, GeoDataResultModel>();
        }
    }
}
