using System;
using System.Collections.Generic;
using StashBot.I18n.Strings;
using StashBot.Models;

namespace StashBot.I18n
{
    public class Localization
    {
        public static string GetPhrase(Phrase phrase, string language, string[] replacements = null)
        {
            Dictionary<Phrase, string> phrases = null;
            Dictionary<Phrase, string> defaultPhrases = EnStrings.StringDictionary;
            string phraseOutput = "(Phrase not found)";
            bool foundPhrase = false;

            switch(language) {
                case "de":
                    phrases = DeStrings.StringDictionary;
                    break;
                case "fr":
                    phrases = FrStrings.StringDictionary;
                    break;
                case "en":
                default:
                    phrases = defaultPhrases;
                    break;
            }

            if (phrases.ContainsKey(phrase))
            {
                phrases.TryGetValue(phrase, out phraseOutput);
                foundPhrase = true;
            } else {
                if(defaultPhrases.ContainsKey(phrase))
                {
                    defaultPhrases.TryGetValue(phrase, out phraseOutput);
                    foundPhrase = true;
                }
            }

            if(replacements != null)
            {
                int replacementIndex = 0;

                foreach(var replacement in replacements)
                {
                    phraseOutput = phraseOutput.Replace($"[{replacementIndex}]", replacements[replacementIndex]);
                    replacementIndex++;
                }
            }

            if(foundPhrase) {
                return phraseOutput;
            } else {
                throw new Exception("Phrase not found");
            }
        }

        public static string GetPhrase(Phrase phrase, TelegramUser user, string[] replacements = null)
        {
            return GetPhrase(phrase, user.Language, replacements);
        }

        public enum Phrase
        {
            AlreadyBeenQueued = 11,
            Delete = 8,
            DeletedXFromQueue = 3,
            Later = 4,
            Latest = 7,
            LinkContainsNoMedia = 12,
            NothingIsQueued = 1,
            NoPermissionPostQueue = 14,
            NoPermissionRemovePost = 9,
            NoPermissionViewQueue = 2,
            PostSuccessfullyQueued = 10,
            ServiceNotSupported = 13,
            Sooner = 5,
            Soonest = 6
        }
    }
}