using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using GearstashBot.Models;

namespace GearstashBot.Utilities
{
    public class DbUtilities
    {
        // TODO: Compare MD5sum of previous backup so we're not creating useless backups
        public static void BackupDatabase()
        {
            if (!Directory.Exists($"config/backup"))
            {
                Directory.CreateDirectory($"config/backup");
            }

            File.Copy(
                $"config/gearstashbot.db",
                $"config/backup/gearstashbot_{DateTime.Now.ToString("yyyyMMddHHmmss")}.db"
            );
        }

        public static void MigrateDatabase()
        {
            var context = new StashBotDbContext();
            context.Database.Migrate();
        }
    }
}