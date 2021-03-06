using System.Collections.Generic;

namespace GearstashBot.I18n.Strings
{
    public class DeStrings
    {
        public static Dictionary<Localization.Phrase, string> StringDictionary = new Dictionary<Localization.Phrase, string>() {
            { Localization.Phrase.AlreadyBeenQueued, "Dies wurde bereits in die Warteschlange gestellt" },
            { Localization.Phrase.Announcement, "<b>Ankündigung</b><br />[0]" }, // DeepL [En->De] 31-Jan-2021
            { Localization.Phrase.Author, "Autor" },
            { Localization.Phrase.Back, "Zurück" },
            { Localization.Phrase.CannotDeleteXFromChannel, "#[0] kann nicht aus dem Kanal gelöscht werden"},
            { Localization.Phrase.CannotFetchFile, "Telegram kann diese Datei nicht herunterladen (<code>[1]</code>):<br />[0]"},
            { Localization.Phrase.CannotFindAuthor, "Kann <code>[0]</code> nicht finden" },
            { Localization.Phrase.CannotFindMessageX, "Nachricht #<code>[0]</code> kann nicht gefunden werden"},
            { Localization.Phrase.CannotDeleteTemporarilyDueToLongRunningRequest, "Dieses Element kann aufgrund einer lang laufenden Anforderung nicht gelöscht werden. Versuchen Sie es in wenigen Sekunden erneut."},
            { Localization.Phrase.CannotPostTemporarilyDueToLongRunningRequest, "Aufgrund einer lang laufenden Anforderung kann kein Beitrag in die Warteschlange gestellt werden. Versuchen Sie es in wenigen Sekunden erneut." },
            { Localization.Phrase.CannotRemovePermissionFromSelf, "Sie können diese Berechtigung nicht von sich selbst entfernen"},
            { Localization.Phrase.Delete, "Löschen" },
            { Localization.Phrase.Deleted, "Gelöscht" },
            { Localization.Phrase.DeleteOthers, "Andere löschen" },
            { Localization.Phrase.DeletedXFromChannel, "#[0] aus dem Kanal gelöscht"},
            { Localization.Phrase.Earlier, "Früher" },
            { Localization.Phrase.Earliest, "Früheste" },
            { Localization.Phrase.FlushDanglingUsers, "Nutzlose Benutzer löschen" },
            { Localization.Phrase.FlushedXDanglingUsers, "[0] nutzlose Benutzer gelöscht" },
            { Localization.Phrase.InvalidArgs, "Ungültige Argumente" },
            { Localization.Phrase.Language, "Sprache" }, // DeepL [En->De] 24-Jan-2021
            { Localization.Phrase.LastAccessed, "Letzter Zugriff" }, // DeepL [En->De] 24-Jan-2021
            { Localization.Phrase.Later, "Später" },
            { Localization.Phrase.Latest, "Aktuellste" },
            { Localization.Phrase.LinkContainsNoMedia, "Dieser Link enthält keine Medien oder existiert nicht" },
            { Localization.Phrase.LoadingQueue, "Lade-Warteschlange..." },
            { Localization.Phrase.ManageUsers, "Benutzer verwalten" },
            { Localization.Phrase.MessageID, "Nachrichten ID" },
            { Localization.Phrase.Name, "Name" },
            { Localization.Phrase.NoDanglingUsers, "Keine nutzlosen Benutzer zum Löschen" },
            { Localization.Phrase.NotSet, "Nicht eingestellt" },
            { Localization.Phrase.NothingIsDeleted, "Nichts wurde gelöscht" },
            { Localization.Phrase.NothingIsPosted, "Es wurde nichts gepostet" },
            { Localization.Phrase.NothingIsQueued, "Nichts steht in der Warteschlange" },
            { Localization.Phrase.NoPermissionFlushDanglingUsers, "Sie haben keine Berechtigung, nutzlose Benutzer zu löschen" },
            { Localization.Phrase.NoPermissionManageUsers, "Sie haben keine Berechtigung zum Verwalten von Benutzern" },
            { Localization.Phrase.NoPermissionPostQueue, "Sie haben keine Berechtigung, neue Beiträge in die Warteschlange zu stellen" },
            { Localization.Phrase.NoPermissionRandomizeQueue, "Sie haben nicht die Berechtigung, die Warteschlange zu mischen"}, // DeepL [En->De] 24-Jan-2021
            { Localization.Phrase.NoPermissionRemovePost, "Sie haben keine Berechtigung, diesen Beitrag zu entfernen" },
            { Localization.Phrase.NoPermissionTools, "Sie haben keine Berechtigung zur Verwendung von Tools" },
            { Localization.Phrase.NoPermissionViewQueue, "Sie haben keine Berechtigung zum Anzeigen der Warteschlange" },
            { Localization.Phrase.PostSuccessfullyQueued, "Beitrag erfolgreich in die Warteschlange gestellt" },
            { Localization.Phrase.Posted, "Geschrieben" },
            { Localization.Phrase.Posts, "Beiträge" },
            { Localization.Phrase.ProfileUpdated, "Profil aktualisiert"},
            { Localization.Phrase.Queue, "Warteschlange" },
            { Localization.Phrase.Queued, "Warteschlange"},
            { Localization.Phrase.RandomizeQueue, "Mischen"}, // DeepL [En->De] 24-Jan-2021
            { Localization.Phrase.RandomizedQueue, "Gemischte Warteschlange" }, // DeepL [En->De] 24-Jan-2021
            { Localization.Phrase.Retry, "Wiederholen" },
            { Localization.Phrase.ServiceNotSupported, "Dieser Dienst wird nicht unterstützt" },
            { Localization.Phrase.Sooner, "Bald" },
            { Localization.Phrase.Soonest, "Demnächst" },
            { Localization.Phrase.Tools, "Werkzeuge" },
            { Localization.Phrase.User, "Benutzer" },
            { Localization.Phrase.Username, "Nutzername" },
            { Localization.Phrase.Viewing, "Anzeigen" }
        };
    }
}