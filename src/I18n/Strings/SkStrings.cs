using System.Collections.Generic;

namespace GearstashBot.I18n.Strings
{
    public class SkStrings
    {
        public static Dictionary<Localization.Phrase, string> StringDictionary = new Dictionary<Localization.Phrase, string>() {
            { Localization.Phrase.AlreadyBeenQueued, "Toto už bolo zaradené do poradia " }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Author, "Plagát" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Back, "Späť" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.CannotDeleteXFromChannel, "Položku #[0] sa nedá odstrániť z kanála "}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.CannotFetchFile, "Telegram nemôže načítať tento súbor (<code>[1]</code>):<br />[0]"}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.CannotFindAuthor, "Nemôžem nájsť <code>[0]</code>"}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.CannotFindMessageX, "Nemožno nájsť správu #<code>[0]</code>"}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.CannotDeleteTemporarilyDueToLongRunningRequest, "Túto položku nie je možné odstrániť z dôvodu dlhodobej požiadavky. Skúste to znova o niekoľko sekúnd. "}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.CannotPostTemporarilyDueToLongRunningRequest, "Z dôvodu dlhodobej požiadavky sa nedá zaradiť do poradia. Skúste to znova o niekoľko sekúnd."}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Delete, "Odstrániť" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Deleted, "Vymazané" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.DeletedXFromChannel, "Odstránené #[0] z kanála"}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Earlier, "Skôr" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Earliest, "Najskôr" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.InvalidArgs, "Neplatné argumenty" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Later, "Neskôr" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Latest, "Najnovšie" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.LinkContainsNoMedia, "Tento odkaz neobsahuje žiadne médiá alebo neexistuje" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.LoadingQueue, "Načítanie fronty ..." }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.MessageID, "ID správy" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.NotSet, "Nie je nastavené"}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.NothingIsPosted, "Nič nebolo zverejnené" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.NothingIsQueued, "Nič nie je v rade " }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.NoPermissionManageUsers, "Nemáte povolenie na správu používateľov"}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.NoPermissionPostQueue, "Nemáte povolenie na zaradenie nových príspevkov do poradia " }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.NoPermissionRandomizeQueue, "Nemáte povolenie na zamiešanie poradia "}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.NoPermissionRemovePost, "You do not have permission to remove this post" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.NoPermissionViewQueue, "Na odstránenie tohto príspevku nemáte povolenie "}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.PostSuccessfullyQueued, "Príspevok bol úspešne zaradený do poradia " }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Posted, "Zverejnené" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Posts, "Príspevky" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Queue, "Fronta" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Queued, "V poradí"}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.RandomizeQueue, "Premiešanie"}, // Ducky 26-Jan-2021
            { Localization.Phrase.RandomizedQueue, "Premiešanie fronta"}, // Ducky 26-Jan-2021
            { Localization.Phrase.Retry, "Nový pokus" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.ServiceNotSupported, "Táto služba nie je podporovaná"}, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Sooner, "Skôr" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Soonest, "Skoro" }, // GTranslate [En->Sk] 26-Jan-2021
            { Localization.Phrase.Viewing, "Prezeranie" }, // GTranslate [En->Sk] 26-Jan-2021
        };
    }
}