using AutoMapper;
using Wards.Domain.Entities;

namespace Wards.Infrastructure.AutoMapper
{
    public sealed class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Usuario, UsuarioInput>().ReverseMap();
        }
    }
}
