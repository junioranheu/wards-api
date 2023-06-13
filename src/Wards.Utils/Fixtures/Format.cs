namespace Wards.Utils.Fixtures
{
    public static class Format
    {
        /// <summary>
        /// Formatar bytes para B, KB, MB, etc;
        /// </summary>
        public static string FormatarBytes(long bytes)
        {
            string[] Suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;

            for (i = 0; i < Suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }

            return string.Format("{0:0.##} {1}", dblSByte, Suffix[i]);
        }

        /// <summary>
        /// Concatena uma data que já é DateTime para o formato dd/MM/yyyy hh;
        /// PS: Esse método é algo muito específico de um projeto em questão...
        /// E muito dificilmente vai ter utilidade em outro local;
        /// </summary>
        public static string FormatarDataExport(DateTime data)
        {
            return data.ToString("dd/MM/yyyy hh");
        }
    }
}