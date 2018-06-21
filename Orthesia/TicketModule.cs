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
            else if (!(await Context.Guild.GetTextChannelsAsync()).ToArray().Any(x => x.Name == "support-" + Context.User.Id))
            {
                string id;
                IReadOnlyCollection<ITextChannel> chans = await Context.Guild.GetTextChannelsAsync();
                do
                {
                    id = GetRandomId();
                } while (chans.Count(x => x.Name == "support-" + id) > 0);
                File.WriteAllText("Saves/chan-" + Context.User.Id + ".dat", id);
                if (File.Exists("Saves/timer-" + Context.User.Id + ".dat"))
                    File.Delete("Saves/timer-" + Context.User.Id + ".dat");
                ITextChannel chan = await Context.Guild.CreateTextChannelAsync("support-" + id);
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
            string userId = await GetChannelId(Context.Channel as ITextChannel);
            if (userId != null && File.Exists("Saves/chan-" + userId + ".dat") && Context.Channel.Name == "support-" + File.ReadAllText("Saves/chan-" + userId + ".dat"))
            {
                IUserMessage msg = await ReplyAsync(Sentences.deleteConfirm);
                await msg.AddReactionAsync(new Emoji("✅"));
                await msg.AddReactionAsync(new Emoji("❌"));
                File.WriteAllText("Saves/support-" + userId + ".dat", msg.Id.ToString());
            }
            else
                await ReplyAsync(Sentences.wrongChan);
        }

        public static async Task<string> GetChannelId(ITextChannel chan)
        {
            string userId = null;
            IEnumerable<IUser> users = await chan.GetUsersAsync().FlattenAsync();
            foreach (string file in Directory.GetFiles("Saves"))
            {
                FileInfo fi = new FileInfo(file);
                if (fi.Name.StartsWith("chan-"))
                {
                    string name = fi.Name.Substring(5, fi.Name.Length - 9);
                    IUser user = users.ToList().Find(x => x.Id.ToString() == name);
                    if (user != null)
                        userId = user.Id.ToString();
                }
            }
            return (userId);
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