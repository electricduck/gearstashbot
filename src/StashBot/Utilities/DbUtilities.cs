using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using StashBot.Models;

namespace StashBot.Utilities
{
    public class DbUtilities
    {
        // TODO: Compare MD5sum of previous backup so we're not creating useless backups
        public static void BackupDatabase()
        {
            if (!Directory.Exists($"{AppArguments.ConfigDirectory}/backup"))
            {
                Directory.CreateDirectory($"{AppArguments.ConfigDirectory}/backup");
            }

            File.Copy(
                $"{AppArguments.ConfigDirectory}/stashbot.db",
                $"{AppArguments.ConfigDirectory}/backup/stashbot_{DateTime.Now.ToString("yyyyMMddHHmmss")}.db"
            );
        }

        public static void MigrateDatabase()
        {
            var context = new StashBotDbContext();
            context.Database.Migrate();
        }
    }
}