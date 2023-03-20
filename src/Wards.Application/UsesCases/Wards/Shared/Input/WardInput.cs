﻿using System.Text.Json.Serialization;

namespace Wards.Application.UsesCases.Wards.Shared.Input
{
    public sealed class WardInput
    {
        public string? Conteudo { get; set; }

        [JsonIgnore]
        public int? UsuarioId { get; set; }
    }
}
