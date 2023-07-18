using AutoMapper;
using Wards.Application.UseCases.Auxiliares.ListarEstado.Shared.Output;
using Wards.Application.UseCases.Feriados.Shared.Models.Input;
using Wards.Application.UseCases.Feriados.Shared.Models.Output;
using Wards.Application.UseCases.FeriadosDatas.Shared.Output;
using Wards.Application.UseCases.FeriadosEstados.Shared.Output;
using Wards.Application.UseCases.Logs.Shared.Input;
using Wards.Application.UseCases.Logs.Shared.Output;
using Wards.Application.UseCases.NewsLettersCadastros.Shared.Input;
using Wards.Application.UseCases.NewsLettersCadastros.Shared.Output;
using Wards.Application.UseCases.Tokens.Shared.Input;
using Wards.Application.UseCases.Usuarios.Shared.Input;
using Wards.Application.UseCases.Usuarios.Shared.Output;
using Wards.Application.UseCases.UsuariosRoles.Shared.Output;
using Wards.Application.UseCases.Wards.Shared.Input;
using Wards.Application.UseCases.Wards.Shared.Output;
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

            CreateMap<FeriadoData, FeriadoDataOutput>();

            CreateMap<FeriadoEstado, FeriadoEstadoOutput>();

            CreateMap<NewsLetterCadastroInput, NewsLetterCadastro>();
            CreateMap<NewsLetterCadastro, NewsLetterCadastroOutput>();

            // Auxiliares;
            CreateMap<Estado, EstadoOutput>();
        }
    }
}