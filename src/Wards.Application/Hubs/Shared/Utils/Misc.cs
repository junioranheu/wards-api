namespace Wards.Application.Hubs.Shared.Utils
{
    internal sealed class Misc
    {
        public static bool IsObjetoValido(object? item)
        {
            try
            {
                if (string.IsNullOrEmpty(item?.ToString()) || item?.ToString() == "undefined")
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string ConverterObjetoParaString(object? item)
        {
            try
            {
                return item?.ToString() ?? string.Empty;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}