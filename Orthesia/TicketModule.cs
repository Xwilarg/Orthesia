﻿using Discord;
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
                await Program.P.db.AddTicket(Context.User.Id, Context.Channel.Id, (await ReplyAsync(Sentences.chanCreated("<#" + chan.Id + ">"))).Id);
                await chan.SendMessageAsync(Sentences.openRequestChan);
                await Context.Message.DeleteAsync();
            }
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
                finalStr += Program.P.rand.Next(10);
            return (finalStr);
        }
    }
}