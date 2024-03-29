﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Wards.Domain.Enums;
using static Wards.Utils.Fixtures.Get;

namespace Wards.Domain.Entities
{
    public sealed class UsuarioRole
    {
        [Key]
        public int UsuarioRoleId { get; set; }

        public int UsuarioId { get; set; }
        [JsonIgnore]
        public Usuario? Usuarios { get; set; }

        public UsuarioRoleEnum RoleId { get; set; }
        public Role? Roles { get; set; }

        public DateTime Data { get; set; } = GerarHorarioBrasilia();
    }
}