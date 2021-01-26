using System.Collections.Generic;

namespace GearstashBot.I18n.Strings
{
    public class FrStrings
    {
        public static Dictionary<Localization.Phrase, string> StringDictionary = new Dictionary<Localization.Phrase, string>() {
            { Localization.Phrase.AlreadyBeenQueued, "Cela a déjà été mis en file d'attente" },
            { Localization.Phrase.Author, "Auteur" },
            { Localization.Phrase.Back, "Retour" },
            { Localization.Phrase.CannotDeleteXFromChannel, "Ne peut pas supprimer le #[0] de la chaîne"},
            { Localization.Phrase.CannotFetchFile, "Telegram ne peut pas télécharger ce fichier (<code>[1]</code>):<br />[0]"},
            { Localization.Phrase.CannotFindAuthor, "Impossible de trouver <code>[0]</code>" },
            { Localization.Phrase.CannotFindMessageX, "Impossible de trouver le message #<code>[0]</code>"},
            { Localization.Phrase.CannotDeleteTemporarilyDueToLongRunningRequest, "Impossible de supprimer cet élément en raison d'une demande de longue durée. Réessayez dans quelques secondes."},
            { Localization.Phrase.CannotPostTemporarilyDueToLongRunningRequest, "Impossible de publier dans la file d'attente en raison d'une demande de longue durée. Réessayez dans quelques secondes."},
            { Localization.Phrase.CannotRemovePermissionFromSelf, "Vous ne pouvez pas supprimer cette autorisation de vous-même"},
            { Localization.Phrase.Delete, "Supprimer" },
            { Localization.Phrase.Deleted, "Effacé" },
            { Localization.Phrase.DeleteOthers, "Supprimer les autres" },
            { Localization.Phrase.DeletedXFromChannel, "Supprimé #[0] de la chaîne"},
            { Localization.Phrase.Earlier, "Plus tôt" },
            { Localization.Phrase.Earliest, "Premier" },
            { Localization.Phrase.FlushDanglingUsers, "Purger les utilisateurs inutiles" },
            { Localization.Phrase.FlushedXDanglingUsers, "Purgé [0] utilisateurs inutiles" },
            { Localization.Phrase.InvalidArgs, "Arguments non valides" },
            { Localization.Phrase.Language, "Langue" }, // DeepL [En->De] 24-Jan-2021
            { Localization.Phrase.LastAccessed, "Dernier accès" }, // DeepL [En->De] 24-Jan-2021
            { Localization.Phrase.Later, "Plus tard" },
            { Localization.Phrase.Latest, "Dernier" },
            { Localization.Phrase.LinkContainsNoMedia, "Ce lien ne contient aucun média ou n'existe pas" },
            { Localization.Phrase.LoadingQueue, "Chargement de la file d'attente..." },
            { Localization.Phrase.ManageUsers, "Gérer les utilisateurs" },
            { Localization.Phrase.MessageID, "ID du message" },
            { Localization.Phrase.Name, "Nom" },
            { Localization.Phrase.NoDanglingUsers, "Aucun utilisateur inutile à purger" },
            { Localization.Phrase.NotSet, "Pas encore défini"},
            { Localization.Phrase.NothingIsDeleted, "Rien n'a été supprimé" },
            { Localization.Phrase.NothingIsPosted, "Rien n'a été posté" },
            { Localization.Phrase.NothingIsQueued, "Rien n'est mis en file d'attente" },
            { Localization.Phrase.NoPermissionFlushDanglingUsers, "Vous n'êtes pas autorisé à purger les utilisateurs inutiles" },
            { Localization.Phrase.NoPermissionManageUsers, "Vous n'êtes pas autorisé à gérer les utilisateurs"},
            { Localization.Phrase.NoPermissionPostQueue, "Vous n'êtes pas autorisé à mettre en file d'attente de nouveaux posts" },
            { Localization.Phrase.NoPermissionRandomizeQueue, "Vous n'avez pas la permission de mélanger les files d'attente"}, // DeepL [En->Fr] 24-Jan-2021
            { Localization.Phrase.NoPermissionRemovePost, "Vous n'êtes pas autorisé à supprimer ce post" },
            { Localization.Phrase.NoPermissionTools, "Vous n'êtes pas autorisé à utiliser des outils" },
            { Localization.Phrase.NoPermissionViewQueue, "Vous n'êtes pas autorisé à afficher la file d'attente" },
            { Localization.Phrase.PostSuccessfullyQueued, "Post mis en file d'attente avec succès" },
            { Localization.Phrase.Posted, "Publié" },
            { Localization.Phrase.Posts, "Messages" },
            { Localization.Phrase.ProfileUpdated, "Profil mis à jour"},
            { Localization.Phrase.Queue, "Queue" },
            { Localization.Phrase.Queued, "Attente"},
            { Localization.Phrase.RandomizeQueue, "Mélanger"}, // GTranslate [En->Fr] 24-Jan-2021
            { Localization.Phrase.RandomizedQueue, "La file d'attente a été remaniée"}, // DeepL [En->Fr] 24-Jan-2021
            { Localization.Phrase.Retry, "Réessayez" },
            { Localization.Phrase.ServiceNotSupported, "Ce service n'est pas pris en charge" },
            { Localization.Phrase.Sooner, "Plus tôt" },
            { Localization.Phrase.Soonest, "Premier" },
            { Localization.Phrase.Tools, "Outils" },
            { Localization.Phrase.User, "Utilisateur" },
            { Localization.Phrase.Username, "Identifiant" },
            { Localization.Phrase.Viewing, "Visualisation" }
        };
    }
}