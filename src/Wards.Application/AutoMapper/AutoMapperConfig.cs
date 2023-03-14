using AutoMapper;
using Wards.Application.UsesCases.Auths.Shared.Input;
using Wards.Application.UsesCases.Logs.Shared.Input;
using Wards.Application.UsesCases.Logs.Shared.Output;
using Wards.Application.UsesCases.Tokens.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Application.UsesCases.UsuariosRoles.Shared.Output;
using Wards.Domain.Entities;

namespace Wards.Application.AutoMapper
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Usuario, UsuarioInput>().ReverseMap();
            CreateMap<UsuarioOutput, Usuario>().ReverseMap();
            CreateMap<UsuarioInput, RegistrarInput>().ReverseMap();

            CreateMap<UsuarioRoleOutput, UsuarioRole>().ReverseMap();

            CreateMap<RefreshToken, RefreshTokenInput>().ReverseMap();

            CreateMap<Log, LogInput>().ReverseMap();
            CreateMap<LogOutput, Log>().ReverseMap();
        }
    }
}