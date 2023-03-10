using AutoMapper;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Domain.Entities;

namespace Wards.Application.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Usuario, UsuarioInput>().ReverseMap();
            CreateMap<UsuarioOutput, Usuario>().ReverseMap();
        }
    }
}