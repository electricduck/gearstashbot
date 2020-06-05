using System.Collections.Generic;

namespace StashBot.I18n.Strings
{
    public class DeStrings
    {
        public static Dictionary<Localization.Phrase, string> StringDictionary = new Dictionary<Localization.Phrase, string>() {
            { Localization.Phrase.AlreadyBeenQueued, "Dies wurde bereits in die Warteschlange gestellt" },
            { Localization.Phrase.CannotFindAuthor, "Kann <code>[0]</code> nicht finden" },
            { Localization.Phrase.CreatedNewAuthor, "Neuer Benutzer hat den Bot gestartet, [0] (<code>[1]</code>). Berechtigungen mit <code>/user [1]</code> setzen."},
            { Localization.Phrase.Delete, "Löschen" },
            { Localization.Phrase.DeletedFromQueue, "Aus der Warteschlange gelöscht"},
            { Localization.Phrase.Later, "Später" },
            { Localization.Phrase.Latest, "Aktuellste" },
            { Localization.Phrase.LinkContainsNoMedia, "Dieser Link enthält keine Medien oder existiert nicht" },
            { Localization.Phrase.NothingIsQueued, "Nichts ist in der ausgewählten Warteschlange" },
            { Localization.Phrase.NoPermissionPostQueue, "Sie haben keine Berechtigung, neue Beiträge in die Warteschlange zu stellen" },
            { Localization.Phrase.NoPermissionRemovePost, "Sie haben keine Berechtigung, diesen Beitrag zu entfernen" },
            { Localization.Phrase.NoPermissionViewQueue, "Sie haben keine Berechtigung zum Anzeigen der Warteschlange" },
            { Localization.Phrase.PostSuccessfullyQueued, "Beitrag erfolgreich in die Warteschlange gestellt" },
            { Localization.Phrase.ServiceNotSupported, "Dieser Dienst wird nicht unterstützt" },
            { Localization.Phrase.Sooner, "Bald" },
            { Localization.Phrase.Soonest, "Demnächst" },
            { Localization.Phrase.WelcomeFirstAuthor, $"<b>Willkommen bei StashBot, [0]!</b><br />Setzen Sie Ihre Berechtigungen mit <code>/user [0]</code> (oder nur <code>/user</code>)." }
        };
    }
}