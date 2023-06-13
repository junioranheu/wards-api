namespace Wards.Utils.Fixtures
{
    public static class Encrypt
    {
        /// <summary>
        /// Criptografar senha (Nuget BCrypt.Net-Next);
        /// </summary>
        public static string Criptografar(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        /// <summary>
        /// Verificar senha (Nuget BCrypt.Net-Next);
        /// </summary>
        public static bool VerificarCriptografia(string senha, string senhaCriptografada)
        {
            if (!BCrypt.Net.BCrypt.Verify(senha, senhaCriptografada))
            {
                return false;
            }

            return true;
        }
    }
}