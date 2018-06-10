using Discord.Commands;
using System.Threading.Tasks;

namespace Orthesia
{
    public class CommunicationModule : ModuleBase
    {
        [Command("Hi"), Summary("Answer with hi"), Alias("Hey", "Hello", "Hi!", "Hey!", "Hello!")]
        public async Task SayHi()
        {
            await ReplyAsync(Sentences.hiStr);
        }
    }
}