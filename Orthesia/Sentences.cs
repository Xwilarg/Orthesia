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
        public readonly static string openRequestChan = "```fix" + Environment.NewLine +
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
    }
}