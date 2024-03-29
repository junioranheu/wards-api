﻿using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Wards.Utils.Fixtures
{
    public static partial class Validate
    {
        /// <summary>
        /// Auto-sugestivo;
        /// </summary>
        public static bool ValidarEmail(string? email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            return RegexEmail().IsMatch(email);
        }

        /// <summary>
        /// Valida se a senha do usuário é forte o suficiente verificando requisitos de senha:
        /// #1 - Tem número;
        /// #2 - Tem letra maiúscula;
        /// #3 - Tem pelo menos X caracteres;
        /// #4 - A senha não contém o nome completo, nome de usuário ou e-mail;
        /// </summary>
        public static (bool isValido, string mensagemErro) ValidarSenha(string senha, string nomeCompleto, string nomeUsuario, string email)
        {
            if (string.IsNullOrEmpty(senha))
            {
                return (false, "A senha não pode estar vazia");
            }

            var temNumero = RegexNumero();
            if (!temNumero.IsMatch(senha))
            {
                return (false, "A senha deve conter ao menos um número");
            }

            var temMaiusculo = RegexIsMaiusculo();
            if (!temMaiusculo.IsMatch(senha))
            {
                return (false, "A senha deve conter ao menos uma letra maiúscula");
            }

            int minCaracteres = 6;
            var temXCaracteres = new Regex(@".{" + minCaracteres + ",}");
            if (!temXCaracteres.IsMatch(senha))
            {
                return (false, $"A senha deve conter ao menos {minCaracteres} caracteres");
            }

            string nomeCompletoPrimeiraParte = nomeCompleto.Split(' ')[0].ToLowerInvariant();
            bool isRepeteNomeCompleto = senha.ToLowerInvariant().Contains(nomeCompletoPrimeiraParte);
            if (isRepeteNomeCompleto)
            {
                return (false, "A senha não pode conter o seu primeiro nome");
            }

            bool isRepeteNomeUsuario = senha.ToLowerInvariant().Contains(nomeUsuario.ToLowerInvariant());
            if (isRepeteNomeUsuario)
            {
                return (false, "A senha não pode conter o seu nome de usuário");
            }

            string emailAntesDoArroba = email.Split('@')[0].ToLowerInvariant();
            bool isRepeteEmail = senha.ToLowerInvariant().Contains(emailAntesDoArroba.ToLowerInvariant());
            if (isRepeteEmail)
            {
                return (false, "A senha não pode conter o seu e-mail");
            }

            return (true, string.Empty);
        }

        /// <summary>
        /// Valida se um IFormFile é uma imagem;
        /// </summary>
        public static bool ValidarIFormFile_IsImagem(IFormFile file)
        {
            string[] imageContentTypes = { "image/jpeg", "image/png", "image/gif", "image/bmp" };

            if (!imageContentTypes.Contains(file.ContentType))
            {
                return false;
            }

            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            string fileExtension = Path.GetExtension(file.FileName);

            if (!imageExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            return true;
        }

        [GeneratedRegex("^([\\w-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([\\w-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$")]
        private static partial Regex RegexEmail();

        [GeneratedRegex("[0-9]+")]
        private static partial Regex RegexNumero();

        [GeneratedRegex("[A-Z]+")]
        private static partial Regex RegexIsMaiusculo();
    }
}