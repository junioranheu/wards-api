﻿using Wards.Application.UseCases.Shared.Models.Input;
using Wards.Application.UseCases.Usuarios.Shared.Output;

namespace Wards.Application.UseCases.Usuarios.ListarUsuario
{
    public interface IListarUsuarioUseCase
    {
        Task<IEnumerable<UsuarioOutput>> Execute(PaginacaoInput input);
    }
}