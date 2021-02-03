using System;
using System.Collections.Generic;
using GearstashBot.I18n.Strings;
using GearstashBot.Models;

namespace GearstashBot.I18n
{
    public class Localization
    {
        // TODO: Use the code from Gearstash to replace this
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
                case "nl":
                    phrases = NlStrings.StringDictionary;
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

            phraseOutput = phraseOutput
                .Replace("<br />", $"{Environment.NewLine}");

            if(foundPhrase) {
                return phraseOutput;
            } else {
                throw new Exception("Phrase not found.");
            }
        }

        public static string GetPhrase(Phrase phrase, TelegramUser user, string[] replacements = null)
        {
            return GetPhrase(phrase, user.Language, replacements);
        }

        public enum Phrase
        {   // Next: 74
            AlreadyBeenQueued = 11,
            Announce = 71,
            Announcement = 72,
            Author = 40,
            Back = 60,
            CannotDeleteXFromChannel = 22,
            CannotFetchFile = 54,
            CannotFindAuthor = 17,
            CannotFindMessageX = 24,
            CannotDeleteTemporarilyDueToLongRunningRequest = 65,
            CannotPostTemporarilyDueToLongRunningRequest = 57,
            CannotRemovePermissionFromSelf = 32,
            CreatedNewAuthor = 15,
            Delete = 8,
            Deleted = 59,
            DeleteOthers = 34,
            DeletedXFromChannel = 21,
            Earlier = 18,
            Earliest = 19,
            FlushDanglingUsers = 44,
            FlushedXDanglingUsers = 50,
            InvalidArgs = 53,
            Language = 69,
            LastAccessed = 70,
            Later = 4,
            Latest = 7,
            LoadingQueue = 20,
            LinkContainsNoMedia = 12,
            ManageUsers = 36,
            MessageID = 41,
            Name = 27,
            NoDanglingUsers = 49,
            NotSet = 25,
            NothingIsDeleted = 63,
            NothingIsPosted = 23,
            NothingIsQueued = 1,
            NoPermissionFlushDanglingUsers = 51,
            NoPermissionManageUsers = 31,
            NoPermissionPostQueue = 14,
            NoPermissionRandomizeQueue = 56,
            NoPermissionRemovePost = 9,
            NoPermissionTools = 46,
            NoPermissionViewQueue = 2,
            PostSuccessfullyQueued = 10,
            Posted = 38,
            Posts = 30,
            ProfileUpdated = 29,
            Queue = 33,
            Queued = 37,
            RandomizeQueue = 55,
            RandomizedQueue = 58,
            Retry = 61,
            ServiceNotSupported = 13,
            Sooner = 5,
            Soonest = 6,
            SqlRowsUpdated = 73,
            Tools = 45,
            User = 26,
            Username = 28,
            Viewing = 62,
            WelcomeFirstAuthor = 16
        }
    }
}