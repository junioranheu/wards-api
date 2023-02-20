using AutoMapper;
using Wards.Domain.DTOs;
using Wards.Domain.Entities;

namespace Wards.Infrastructure.AutoMapper
{
    public sealed class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Usuario, UsuarioDTO>().ReverseMap();
        }
    }
}
