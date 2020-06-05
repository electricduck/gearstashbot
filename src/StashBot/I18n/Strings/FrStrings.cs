using System.Collections.Generic;

namespace StashBot.I18n.Strings
{
    public class FrStrings
    {
        public static Dictionary<Localization.Phrase, string> StringDictionary = new Dictionary<Localization.Phrase, string>() {
            { Localization.Phrase.AlreadyBeenQueued, "Cela a déjà été mis en file d'attente" },
            { Localization.Phrase.Delete, "Supprimer" },
            { Localization.Phrase.DeletedXFromQueue, "#[0] supprimé de la file d'attente"},
            { Localization.Phrase.Later, "Plus tard" },
            { Localization.Phrase.Latest, "Dernier" },
            { Localization.Phrase.LinkContainsNoMedia, "Ce lien ne contient aucun média ou n'existe pas" },
            { Localization.Phrase.NothingIsQueued, "Rien n'est mis en file d'attente" },
            { Localization.Phrase.NoPermissionPostQueue, "Vous n'êtes pas autorisé à mettre en file d'attente de nouveaux posts" },
            { Localization.Phrase.NoPermissionRemovePost, "Vous n'êtes pas autorisé à supprimer ce post" },
            { Localization.Phrase.NoPermissionViewQueue, "Vous n'êtes pas autorisé à afficher la file d'attente" },
            { Localization.Phrase.PostSuccessfullyQueued, "Post mis en file d'attente avec succès" },
            { Localization.Phrase.ServiceNotSupported, "Ce service n'est pas pris en charge" },
            { Localization.Phrase.Sooner, "Plus tôt" },
            { Localization.Phrase.Soonest, "Premier" }
        };
    }
}