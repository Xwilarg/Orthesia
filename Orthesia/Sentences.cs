using System;

namespace Orthesia
{
    public static class Sentences
    {
        public readonly static ulong myId = Program.P.client.CurrentUser.Id;

        public readonly static string hiStr = "Bonjour";
        public readonly static string deleteConfirm = "Êtes-vous sûr de vouloir fermer la requête ?";
        public readonly static string chanAlreadyExist = "Vous avez déjà ouvert un canal de support.";
        public static string chanCreated(string chanName) { return ("Le canal " + chanName + " a été crée pour vous."); }
        public readonly static string wrongChan = "Vous devez être dans un canal de support pour le fermer.";
        public readonly static string needWait = "Vous devez attendre 10 minutes entre chaque ticket.";
        public readonly static string help = "**Ticket**: Ouvre un ticket dans un canal privé." + Environment.NewLine +
                                            "**Close**: Ferme le ticket couramment ouvert.";
        public readonly static string openRequestChan =
            "```fix" + Environment.NewLine +
            "Vous venez de créer un ticket, merci de choisir la nature de votre ticket." + Environment.NewLine +
            Environment.NewLine +
            "• Choisissez 1 si vous souhaitez signaler un joueur." + Environment.NewLine +
            "• Choisissez 2 si vous souhaitez signaler un bug en jeu." + Environment.NewLine +
            "• Choisissez 3 si vous avez un problème avec la boutique." + Environment.NewLine +
            "• Choisissez 4 si vous avez une question ou une autre demande particulière." + Environment.NewLine +
            Environment.NewLine +
            "Sachez que si votre ticket ne respecte pas les règles, nous nous réservons le droit de le supprimer." + Environment.NewLine +
            "Consultez toujours les règles afin de vous assurer de pas signaler inutilement." + Environment.NewLine +
            Environment.NewLine +
            "Le lien est disponible ici : https://orthesia4.webnode.fr/reglement/" + Environment.NewLine +
            "```";
        public readonly static string category1 =
            "```" + Environment.NewLine +
            "TICKET DE CATÉGORIE 1 : Signaler un joueur" + Environment.NewLine +
            Environment.NewLine +
            "Merci de nous fournir ces éléments suivants afin que nous puissions sanctionner le joueur :" + Environment.NewLine +
            Environment.NewLine +
            "1. Images / Vidéos obligatoires" + Environment.NewLine +
            "2. Son pseudonyme" + Environment.NewLine +
            Environment.NewLine +
            "Nos équipes sanctionneront ce joueur dans les meilleurs délais, nous vous remercions de votre patience." + Environment.NewLine +
            "```";
        public readonly static string category2 =
            "```" + Environment.NewLine +
            "TICKET DE CATÉGORIE 2 : Problème en jeu" + Environment.NewLine +
            Environment.NewLine +
            "Merci de nous fournir ces éléments suivants afin que nous puissions résoudre votre problème :" + Environment.NewLine +
            Environment.NewLine +
            "1. Description du problème" + Environment.NewLine +
            "2. La procédure pour reproduire le bug" + Environment.NewLine +
            "3. Le code d'erreur / Images / Vidéos du bug" + Environment.NewLine +
            Environment.NewLine +
            "Nos équipes vous répondront dans les meilleurs délais, nous vous remercions de votre patience." + Environment.NewLine +
            "```";
        public readonly static string category3 =
            "```" + Environment.NewLine +
            "TICKET DE CATÉGORIE 3 : Problème avec la boutique" + Environment.NewLine +
            Environment.NewLine +
            "Merci de nous fournir cet élément suivant afin que nous puissions résoudre votre problème :" + Environment.NewLine +
            Environment.NewLine +
            "1. Description du problème" + Environment.NewLine +
            Environment.NewLine +
            "Nos équipes vous répondront dans les meilleurs délais, nous vous remercions de votre patience." + Environment.NewLine +
            "```";
        public readonly static string category4 =
            "```" + Environment.NewLine +
            "TICKET DE CATÉGORIE 4 : Demande particulière" + Environment.NewLine +
            Environment.NewLine +
            "Merci de nous spécifier votre demande." + Environment.NewLine +
            Environment.NewLine +
            "Nos équipes vous répondront les meilleurs délais, nous vous remercions de votre patience." + Environment.NewLine +
            "```";
    }
}