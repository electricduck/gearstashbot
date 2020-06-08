using System;
using System.IO;
using StashBot.Utilities;

namespace StashBot.Utilities
{
    public class DbUtilities
    {
        public static void BackupDatabase()
        {
            if (!Directory.Exists("_backup"))
            {
                Directory.CreateDirectory("_backup");
            }

            File.Copy(
                "stashbot.db",
                $"_backup/stashbot_{DateTime.Now.ToString("yyyyMMddHHmmss")}.db"
            );
        }
    }
}