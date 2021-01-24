using System;
using System.Reflection;
using GearstashBot.Models;

namespace GearstashBot.Utilities
{
    public class ReflectionUtilities
    {
        public static string GetEnvironment()
        {
            var v = Environment.Version;
            var n = (v.Major >= 5) ? ".NET" : ".NET Core";
            return $"{n} {v.Major}.{v.Minor}";
        }

        public static AppVersion GetVersion()
        {
            AppVersion version = new AppVersion();
            var attribute = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            if(attribute?.InformationalVersion != null)
            {
                var versionString = attribute.InformationalVersion;
                
                version.Commit = versionString.Substring(versionString.IndexOf('+') + 1);
            }

            return version;

            //var v = Assembly.GetEntryAssembly().GetName().Version;
            //return $"{v.Major}.{v.Minor}.{v.Build}";
        }

        public static string GetVersionString()
        {
            AppVersion version = GetVersion();
            return $"{version.Commit}";
        }
    }
}