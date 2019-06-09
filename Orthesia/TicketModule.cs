using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Orthesia
{
    public class TicketModule : ModuleBase
    {
        [Command("Ticket")]
        public async Task OpenTicket()
        {
            if (await Program.P.db.DoesTicketExist(Context.User.Id, Context.Guild))
                await ReplyAsync(Sentences.chanAlreadyExist);
            else if(!await Program.P.db.IsLastMoreThan10Minutes(Context.User.Id))
                await ReplyAsync(Sentences.needWait);
            else
            {
                string id;
                IReadOnlyCollection<ITextChannel> chans = await Context.Guild.GetTextChannelsAsync();
                do
                {
                    id = GetRandomId();
                } while (chans.Count(x => x.Name == "support-" + id) > 0);
                ITextChannel chan = await Context.Guild.CreateTextChannelAsync("support-" + id, x => x.CategoryId = 585811641648807936);
                await chan.AddPermissionOverwriteAsync(Context.User, new OverwritePermissions(viewChannel: PermValue.Allow));
                await chan.AddPermissionOverwriteAsync(Context.Guild.GetRole(455505689612255243), new OverwritePermissions(viewChannel: PermValue.Allow));
                await chan.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(viewChannel: PermValue.Deny));
                IUserMessage msg = await chan.SendMessageAsync(Sentences.openRequestChan);
                await msg.AddReactionsAsync(new[] { new Emoji("1⃣"), new Emoji("2⃣"), new Emoji("3⃣"), new Emoji("4⃣") });
                await Program.P.db.AddTicket(Context.User.Id, chan.Id, (await ReplyAsync(Sentences.chanCreated("<#" + chan.Id + ">"))).Id, msg.Id, id, Context.Channel.Id);
                await Context.Message.DeleteAsync();
            }
        }

        [Command("Close")]
        public async Task CloseTicket()
        {
            string id = (Context.Channel.Name.StartsWith("support-")) ? (Context.Channel.Name.Substring(8, 4)) : (null);
            if (await Program.P.db.DoesTicketExist(Context.User.Id, Context.Guild))
            {
                await Program.P.db.DeleteCloseMsg(Context.User.Id, Context.Channel);
                IUserMessage msg = await ReplyAsync(Sentences.deleteConfirm);
                await msg.AddReactionAsync(new Emoji("✅"));
                await msg.AddReactionAsync(new Emoji("❌"));
                await Program.P.db.UpdateCloseMsg(Context.User.Id, msg.Id);
            }
            else
                await ReplyAsync(Sentences.wrongChan);
        }

        private string GetRandomId()
        {
            string finalStr = "";
            for (int i = 0; i < 4; i++)
                finalStr += Program.P.rand.Next(10);
            return (finalStr);
        }
    }
}