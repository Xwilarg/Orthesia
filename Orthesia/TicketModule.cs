using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Orthesia
{
    public class TicketModule : ModuleBase
    {
        [Command("Ticket")]
        public async Task OpenTicket()
        {
            if (!(await Context.Guild.GetTextChannelsAsync()).ToArray().Any(x => x.Name == "support-" + Context.User.Id))
            {
                ITextChannel chan = await Context.Guild.CreateTextChannelAsync("support-" + Context.User.Id);
                await chan.AddPermissionOverwriteAsync(Context.User, new OverwritePermissions(readMessages: PermValue.Allow));
                await chan.AddPermissionOverwriteAsync(Context.Guild.GetRole(455505689612255243), new OverwritePermissions(readMessages: PermValue.Allow));
                await chan.AddPermissionOverwriteAsync(Context.Guild.EveryoneRole, new OverwritePermissions(readMessages: PermValue.Deny));
            }
        }

        [Command("Close")]
        public async Task CloseTicket()
        {
            ITextChannel chan = (await Context.Guild.GetTextChannelsAsync()).ToList().Find(x => x.Name == "support-" + Context.User.Id);
            if (chan != null)
                await chan.DeleteAsync();
        }
    }
}