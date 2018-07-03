using System;

namespace Orthesia
{
    public static class Sentences
    {
        public readonly static ulong myId = Program.p.client.CurrentUser.Id;

        public readonly static string hiStr = "Bonjour";
        public readonly static string deleteConfirm = "Êtes-vous sûr de vouloir fermer la requête ?";
        public readonly static string chanAlreadyExist = "Vous avez déjà ouvert un canal de support.";
        public static string chanCreated(string chanName) { return ("Le canal " + chanName + " a été crée pour vous."); }
        public readonly static string wrongChan = "Vous devez être dans un canal de support pour le fermer.";
        public readonly static string needWait = "Vous devez attendre 10 minutes entre chaque ticket.";
        public readonly static string help = "**Ticket**: Ouvre un ticket dans un canal privé." + Environment.NewLine +
                                            "**Close**: Ferme le ticket couramment ouvert.";
        public readonly static string openRequestPm = "Vous avez crée une requête sur le Discord Orthesia, un membre du staff répondra à votre demande au plus vite.";
        public readonly static string requestClosedPm = "Vous avez fermé la requête avec succès.";
        public readonly static string openRequestChan = "```fix" + Environment.NewLine +
                                                        "Merci d'avoir crée une requête sur le Discord d'Orthesia." + Environment.NewLine +
                                                        "Un membre du support répondra à vos questions dans les meilleurs délais." + Environment.NewLine +
                                                        "En attendant, veuillez nous décrire la raison de cette requête en donnant un maximum d'informations possibles." + Environment.NewLine + Environment.NewLine +
                                                        "• Si vous signalez un joueur pour une raison particulière, assurez-vous d'avoir les preuves nécessaires;" + Environment.NewLine +
                                                        "• Si vous signalez un bug ou un quelconque problème, fournissez-nous un maximum d'informations dessus;" + Environment.NewLine +
                                                        "• Pour toute autre demande, soyez clairs sur vos demandes." + Environment.NewLine + Environment.NewLine +
                                                        "Nous nous réservons le droit de fermer un ticket si celui-ci ne respecte pas les règles ou qu'il est jugé inutile." + Environment.NewLine +
                                                        "Consultez toujours les règles afin de vous assurer de ne pas signaler inutilement, https://lc.cx/mnzE" + Environment.NewLine +
                                                        "```";
    }
}