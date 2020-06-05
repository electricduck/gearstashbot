using System.Reflection;

namespace StashBot.Utilities
{
    public class ReflectionUtilities
    {
        public static string GetVersion()
        {
            var v = Assembly.GetEntryAssembly().GetName().Version;
            return $"{v.Major}.{v.Minor}.{v.Build}";
        }
    }
}