using System.ComponentModel.DataAnnotations;

namespace Wards.Domain.Entities
{
    public sealed class CsvImportExemploUsuario
    {
        [Key]
        public int CsvImportExemploUsuarioId { get; set; }

        public string? Identifier { get; set; } = null;

        public string? OneTimePassword { get; set; } = null;

        public string? RecoveryCode { get; set; } = null;

        public string? FirstName { get; set; } = null;

        public string? LastName { get; set; } = null;

        public string? Department { get; set; } = null;

        public string? Location { get; set; } = null;
    }
}