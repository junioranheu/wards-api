using System.Reflection;

namespace Wards.Infrastructure.Registry
{
    public static class EnumRegistry
    {
        private static Dictionary<string, Type> _enums = [];

        static EnumRegistry()
        {
            Refresh();
        }

        public static void Refresh()
        {
            _enums = AppDomain.CurrentDomain.GetAssemblies().
                SelectMany(asm =>
                {
                    try
                    {
                        return asm.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        return ex.Types.Where(t => t is not null);
                    }
                }).
                Where(t => t is not null
                         && t.IsEnum
                         && t.Name.EndsWith("Enum", StringComparison.OrdinalIgnoreCase)
                         && !t.Name.Contains('_')
                         && t.Name != "VarEnum"
                         && t.Name != "ColumnEnum").
                ToDictionary(t => t!.Name, t => t, StringComparer.OrdinalIgnoreCase)!;
        }

        public static bool TryGetEnum(string name, out Type? enumType) => _enums.TryGetValue(name, out enumType);

        public static IEnumerable<string> GetEnumNames() => _enums.Keys.OrderBy(x => x);
    }
}