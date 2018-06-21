using System;

namespace Orthesia
{
    public static class Sentences
    {
        public readonly static ulong myId = Program.p.client.CurrentUser.Id;

        public readonly static string hiStr = "Hi";
        public readonly static string deleteConfirm = "Êtes-vous sûr de vouloir fermer la requête ?";
        public readonly static string chanAlreadyExist = "You already have a opened support channel.";
        public static string chanCreated(string chanName) { return ("The channel " + chanName + " was created for you."); }
        public readonly static string wrongChan = "You must be in your support channel to close it.";
        public readonly static string needWait = "You need to wait at least 10 minutes before opening a new ticket.";
        public readonly static string help = "**Ticket**: Open a ticket in a private channel" + Environment.NewLine +
                                            "**Close**: Close the currently opened ticket";
        public readonly static string openRequestPm = "Vous avez crée une requête sur le Discord Orthesia, un membre du staff répondra à votre demande au plus vite.";
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