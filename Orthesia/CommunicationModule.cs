using Discord.Commands;
using DiscordUtils;
using System.Threading.Tasks;

namespace Orthesia
{
    public class CommunicationModule : ModuleBase
    {
        [Command("Info"), Alias("Botinfo")]
        public async Task SayHi()
        {
            await ReplyAsync("", false, Utils.GetBotInfo(Program.P.StartTime, null, Program.P.client.CurrentUser));
        }

        [Command("Help"), Summary("Give the help"), Alias("Commands")]
        public async Task Help()
        {
            await ReplyAsync(Sentences.help);
        }
    }
}