﻿using Wards.Domain.Entities;

namespace Wards.Application.UsesCases.Usuarios.ObterUsuario
{
    public interface IObterUsuarioUseCase
    {
        Task<Usuario> Obter(int id);
    }
}