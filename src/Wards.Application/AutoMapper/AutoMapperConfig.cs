using AutoMapper;
using Wards.Application.UsesCases.Auxiliares.ListarEstado.Shared.Output;
using Wards.Application.UsesCases.Feriados.Shared.Models.Input;
using Wards.Application.UsesCases.Feriados.Shared.Models.Output;
using Wards.Application.UsesCases.Logs.Shared.Input;
using Wards.Application.UsesCases.Logs.Shared.Output;
using Wards.Application.UsesCases.Tokens.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Input;
using Wards.Application.UsesCases.Usuarios.Shared.Output;
using Wards.Application.UsesCases.UsuariosRoles.Shared.Output;
using Wards.Application.UsesCases.Wards.Shared.Input;
using Wards.Application.UsesCases.Wards.Shared.Output;
using Wards.Domain.Entities;

namespace Wards.Application.AutoMapper
{
    public sealed class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<CriarUsuarioInput, Usuario>();
            CreateMap<Usuario, UsuarioOutput>();
            CreateMap<Usuario, AutenticarUsuarioOutput>();
            CreateMap<UsuarioOutput, AutenticarUsuarioOutput>();

            CreateMap<UsuarioRole, UsuarioRoleOutput>();

            CreateMap<RefreshTokenInput, RefreshToken>();

            CreateMap<LogInput, Log>();
            CreateMap<Log, LogOutput>();

            CreateMap<WardInput, Ward>();
            CreateMap<Ward, WardOutput>();

            CreateMap<FeriadoInput, Feriado>();
            CreateMap<Feriado, FeriadoOutput>();

            // Auxiliares;
            CreateMap<Estado, EstadoOutput>();
        }
    }
}