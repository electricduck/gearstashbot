using System;
using Microsoft.EntityFrameworkCore;
using GearstashBot.Data;
using GearstashBot.I18n;
using GearstashBot.Models.ArgumentModels;
using GearstashBot.Utilities;

namespace GearstashBot.Handlers.CommandHandlers
{
    public class SqlCommandHandler
    {
        // TODO: Detect errors
        public static void Invoke(CommandHandlerArguments arguments, bool reload = false)
        {
            if (AuthorData.CanAuthorExecuteSql(arguments.TelegramUser.Id))
            {
                Exception exception = null;
                bool hasSucceeded;
                int rowsUpdated = 0;

                using (var db = new StashBotDbContext())
                {
                    try
                    {
                        rowsUpdated = db.Database.ExecuteSqlRaw(arguments.CommandArgument);
                        hasSucceeded = true;
                    }
                    catch (Exception e)
                    {
                        exception = e;
                        hasSucceeded = false;
                    }
                }

                if (hasSucceeded)
                {
                    MessageUtilities.SendSuccessMessage(
                        Localization.GetPhrase(
                            Localization.Phrase.SqlRowsUpdated, arguments.TelegramUser,
                            new string[] { 
                                (rowsUpdated <= 0 ) ? "0" : rowsUpdated.ToString()
                            }
                        ),
                        arguments.TelegramMessageEvent
                    );
                }
                else
                {
                    MessageUtilities.SendErrorMessage(exception, arguments.TelegramMessageEvent, true);
                }
            }
        }
    }
}