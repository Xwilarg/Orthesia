using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
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
            else if (!await IsUserInSupport(Context.Guild, Context.User))
            {
                string id;
                IReadOnlyCollection<ITextChannel> chans = await Context.Guild.GetTextChannelsAsync();
                do
                {
                    id = GetRandomId();
                } while (chans.Count(x => x.Name == "support-" + id) > 0);
                if (File.Exists("Saves/timer-" + Context.User.Id + ".dat"))
                    File.Delete("Saves/timer-" + Context.User.Id + ".dat");
                ITextChannel chan = await Context.Guild.CreateTextChannelAsync("support-" + id);
                await chan.AddPermissionOverwriteAsync(Context.User, new OverwritePermissions(readMessages: PermValue.Allow));
                await chan.AddPermissionOverwriteAsync(Context.Guild.GetRole(455505689612255243), new OverwritePermissions(readMessages: PermValue.Allow));
                await chan.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(readMessages: PermValue.Deny));
                File.WriteAllText("Saves/chan-" + id + ".dat", Context.Channel.Id + Environment.NewLine + (await ReplyAsync(Sentences.chanCreated("<#" + chan.Id + ">"))).Id);
                await Context.User.SendMessageAsync(Sentences.openRequestPm);
                await chan.SendMessageAsync(Sentences.openRequestChan);
                await Context.Message.DeleteAsync();
            }
            else
                await ReplyAsync(Sentences.chanAlreadyExist);
        }

        private async Task<bool> IsUserInSupport(IGuild guild, IUser user)
        {
            foreach (ITextChannel chan in await guild.GetTextChannelsAsync())
            {
                if (chan.Name.StartsWith("support-") && await (chan.GetUsersAsync().Flatten()).Any(x => x.Id == user.Id))
                    return (true);
            }
            return (false);
        }

        [Command("Close")]
        public async Task CloseTicket()
        {
            string id = (Context.Channel.Name.StartsWith("support-")) ? (Context.Channel.Name.Substring(8, 4)) : (null);
            if (id != null)
            {
                if (File.Exists("Saves/support-" + id + ".dat"))
                    await (await Context.Channel.GetMessageAsync(Convert.ToUInt64(File.ReadAllText("Saves/support-" + id + ".dat")))).DeleteAsync();
                IUserMessage msg = await ReplyAsync(Sentences.deleteConfirm);
                await msg.AddReactionAsync(new Emoji("✅"));
                await msg.AddReactionAsync(new Emoji("❌"));
                File.WriteAllText("Saves/support-" + id + ".dat", msg.Id.ToString());
            }
            else
                await ReplyAsync(Sentences.wrongChan);
        }

        private string GetRandomId()
        {
            string finalStr = "";
            for (int i = 0; i < 4; i++)
                finalStr += Program.p.rand.Next(10);
            return (finalStr);
        }
    }
}