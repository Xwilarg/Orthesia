using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Orthesia
{
    class Program
    {
        public static void Main(string[] args)
             => new Program().MainAsync().GetAwaiter().GetResult();

        public readonly DiscordSocketClient client;
        private readonly IServiceCollection map = new ServiceCollection();
        private readonly CommandService commands = new CommandService();
        public static Program p;

        private int commandReceived;
        private string lastHourSent;
        public Random rand;

        private Program()
        {
            client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose,
                MessageCacheSize = 50
            });
            client.Log += Log;
            commands.Log += Log;
        }

        private async Task MainAsync()
        {
            p = this;

            lastHourSent = DateTime.Now.ToString("HH");
            if (File.Exists("Saves/CommandReceived.dat"))
            {
                string[] content = File.ReadAllLines("Saves/CommandReceived.dat");
                if (content[1] == lastHourSent)
                    commandReceived = Convert.ToInt32(content[0]);
                else
                    commandReceived = 0;
            }
            else
                commandReceived = 0;

            rand = new Random();

            client.MessageReceived += HandleCommandAsync;
            client.ReactionAdded += ReactionAdded;

            await commands.AddModuleAsync<CommunicationModule>();
            await commands.AddModuleAsync<TicketModule>();

            await client.LoginAsync(TokenType.Bot, File.ReadAllText("Keys/token.dat"));
            await client.StartAsync();

            var task = Task.Run(async () => {
                for (;;)
                {
                    await Task.Delay(60000);
                    UpdateStatus();
                }
            });

            await Task.Delay(-1);
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> cach, ISocketMessageChannel chan, SocketReaction react)
        {
            if (react.UserId == Sentences.myId)
                return;
            string id = (chan.Name.StartsWith("support-")) ? (chan.Name.Substring(8, 4)) : (null);
            if (id != null && File.Exists("Saves/support-" + id + ".dat") && File.ReadAllText("Saves/support-" + id + ".dat") == react.MessageId.ToString())
            {
                if (react.Emote.Name == "✅")
                {
                    File.Delete("Saves/support-" + id + ".dat");
                    string[] content = File.ReadAllLines("Saves/chan-" + id + ".dat");
                    await (await (await (chan as ITextChannel).Guild.GetTextChannelAsync(Convert.ToUInt64(content[0]))).GetMessageAsync(Convert.ToUInt64(content[1]))).DeleteAsync();
                    File.Delete("Saves/chan-" + id + ".dat");
                    File.WriteAllText("Saves/timer-" + react.UserId + ".dat", DateTime.Now.ToString("yyMMddHHmmss"));
                    await (chan as ITextChannel).DeleteAsync();
                    await (await react.User.Value.GetOrCreateDMChannelAsync()).SendMessageAsync(Sentences.requestClosedPm);
                }
                else if (react.Emote.Name == "❌")
                {
                    File.Delete("Saves/support-" + id + ".dat");
                    await react.Message.Value.DeleteAsync();
                }
            }
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            int pos = 0;
            if (msg.HasMentionPrefix(client.CurrentUser, ref pos) || msg.HasStringPrefix("!", ref pos))
            {
                var context = new SocketCommandContext(client, msg);
                IResult result = await commands.ExecuteAsync(context, pos);
                if (result.IsSuccess && !context.User.IsBot)
                {
                    commandReceived++;
                    File.WriteAllText("Saves/CommandReceived.dat", commandReceived + Environment.NewLine + lastHourSent);
                }
            }
        }

        private async void UpdateStatus()
        {
            HttpClient httpClient = new HttpClient();
            var values = new Dictionary<string, string> {
                           { "token", File.ReadAllLines("Keys/websiteToken.dat")[1] },
                           { "name", "Orthesia" }
                        };
            if (lastHourSent != DateTime.Now.ToString("HH"))
            {
                lastHourSent = DateTime.Now.ToString("HH");
                commandReceived = 0;
            }
            values.Add("serverCount", client.Guilds.Count.ToString());
            values.Add("nbMsgs", commandReceived.ToString());
            FormUrlEncodedContent content = new FormUrlEncodedContent(values);

            try
            {
                await httpClient.PostAsync(File.ReadAllLines("Keys/websiteToken.dat")[0], content);
            }
            catch (HttpRequestException)
            { }
            catch (TaskCanceledException)
            { }
        }

        private Task Log(LogMessage msg)
        {
            var cc = Console.ForegroundColor;
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
            }
            Console.WriteLine(msg);
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }
    }
}
