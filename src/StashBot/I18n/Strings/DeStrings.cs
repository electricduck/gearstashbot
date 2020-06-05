using System.Collections.Generic;

namespace StashBot.I18n.Strings
{
    public class DeStrings
    {
        public static Dictionary<Localization.Phrase, string> StringDictionary = new Dictionary<Localization.Phrase, string>() {
            { Localization.Phrase.AlreadyBeenQueued, "Dies wurde bereits in die Warteschlange gestellt" },
            { Localization.Phrase.Delete, "Löschen" },
            { Localization.Phrase.DeletedXFromQueue, "Gelöschte #[0] aus der Warteschlange"},
            { Localization.Phrase.Later, "Später" },
            { Localization.Phrase.Latest, "Aktuellste" },
            { Localization.Phrase.LinkContainsNoMedia, "Dieser Link enthält keine Medien oder existiert nicht" },
            { Localization.Phrase.NothingIsQueued, "Nichts steht in der Warteschlange" },
            { Localization.Phrase.NoPermissionPostQueue, "Sie haben keine Berechtigung, neue Beiträge in die Warteschlange zu stellen" },
            { Localization.Phrase.NoPermissionRemovePost, "Sie haben keine Berechtigung, diesen Beitrag zu entfernen" },
            { Localization.Phrase.NoPermissionViewQueue, "Sie haben keine Berechtigung zum Anzeigen der Warteschlange" },
            { Localization.Phrase.PostSuccessfullyQueued, "Beitrag erfolgreich in die Warteschlange gestellt" },
            { Localization.Phrase.ServiceNotSupported, "Dieser Dienst wird nicht unterstützt" },
            { Localization.Phrase.Sooner, "Bald" },
            { Localization.Phrase.Soonest, "Demnächst" }
        };
    }
}