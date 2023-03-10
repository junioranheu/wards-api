using AutoMapper;

namespace Wards.Application.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<CurvaTipica, CurvaTipicaInput>().ReverseMap();
            CreateMap<CurvaTipicaResponse, CurvaTipica>().ReverseMap();
        }
    }
}