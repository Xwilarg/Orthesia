using Discord;
using Discord.Commands;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Orthesia
{
    public class TicketModule : ModuleBase
    {
        [Command("Ticket")]
        public async Task OpenTicket()
        {
            if (File.Exists("Saves/timer-" + Context.User.Id + ".dat")
                && DateTime.ParseExact(File.ReadAllText("Saves/timer-" + Context.User.Id + ".dat"), "yyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture).AddMinutes(10).CompareTo(DateTime.Now) == 1)
                await ReplyAsync(Sentences.needWait);
            else if (!(await Context.Guild.GetTextChannelsAsync()).ToArray().Any(x => x.Name == "support-" + Context.User.Id))
            {
                if (File.Exists("Saves/timer-" + Context.User.Id + ".dat"))
                    File.Delete("Saves/timer-" + Context.User.Id + ".dat");
                ITextChannel chan = await Context.Guild.CreateTextChannelAsync("support-" + Context.User.Id);
                await chan.AddPermissionOverwriteAsync(Context.User, new OverwritePermissions(readMessages: PermValue.Allow));
                await chan.AddPermissionOverwriteAsync(Context.Guild.GetRole(455505689612255243), new OverwritePermissions(readMessages: PermValue.Allow));
                await chan.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(readMessages: PermValue.Deny));
                await ReplyAsync(Sentences.chanCreated("<#" + chan.Id + ">"));
                await Context.User.SendMessageAsync(Sentences.openRequestPm);
                await chan.SendMessageAsync(Sentences.openRequestChan);
            }
            else
                await ReplyAsync(Sentences.chanAlreadyExist);
        }

        [Command("Close")]
        public async Task CloseTicket()
        {
            if (Context.Channel.Name == "support-" + Context.User.Id)
            {
                IUserMessage msg = await ReplyAsync(Sentences.deleteConfirm);
                await msg.AddReactionAsync(new Emoji("✅"));
                await msg.AddReactionAsync(new Emoji("❌"));
                File.WriteAllText("Saves/support-" + Context.User.Id + ".dat", msg.Id.ToString());
            }
            else
                await ReplyAsync(Sentences.wrongChan);
        }
    }
}