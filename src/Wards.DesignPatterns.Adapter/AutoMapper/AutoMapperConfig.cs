using AutoMapper;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Domain.Entities;

namespace Wards.DesignPatterns.Adapter.AutoMapper
{
    public sealed class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Usuario, UsuarioInput>().ReverseMap();
        }
    }
}
