using System;
using System.Collections.Generic;
using System.Linq;
using StashBot.Handlers.CommandHandlers;
using StashBot.Models;
using StashBot.Models.ReturnModels;

namespace StashBot.Data
{
    public class HelpData
    {
        private static Dictionary<string, Help> HelpDictionary = new Dictionary<string, Help>();

        public static HelpReturn GetHelp(string command)
        {
            HelpReturn returnModel = new HelpReturn { };

            if (HelpDictionary.ContainsKey(command))
            {
                Help helpData = null;
                HelpDictionary.TryGetValue(command, out helpData);

                returnModel.Available = true;
                returnModel.Help = helpData;
            }
            
            return returnModel;
        }

        public static void CompileHelp()
        {
            HelpDictionary.Add("catpls", CatPlsCommandHandler.Help);
            HelpDictionary.Add("info", InfoCommandHandler.Help);
            HelpDictionary.Add("post", PostCommandHandler.Help);
            HelpDictionary.Add("tools", ToolsCommandHandler.Help);
            HelpDictionary.Add("user", UserCommandHandler.Help);
            HelpDictionary.Add("view", ViewCommandHandler.Help);

            string helpContents = "";

            foreach(var helpItem in HelpDictionary)
            {
                helpContents += $"<b>/{helpItem.Value.Command}</b> - <i>{helpItem.Value.Description.Split(new [] { '\r', '\n' }).FirstOrDefault()}{Environment.NewLine}</i>";
            }

            HelpDictionary.Add("help", new Help {
                Command = "help",
                Description = helpContents,
                DescriptionIsItalicized = false
            });

            HelpDictionary.Add("set", UserCommandHandler.Help);
            HelpDictionary.Add("tool", ToolsCommandHandler.Help);
        }
    }
}