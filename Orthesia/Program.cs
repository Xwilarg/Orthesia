using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Orthesia
{
    class Program
    {
        public static void Main(string[] args)
             => new Program().MainAsync().GetAwaiter().GetResult();

        public readonly DiscordSocketClient client;
        private readonly CommandService commands = new CommandService();
        public static Program p;
        
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
            rand = new Random();

            client.MessageReceived += HandleCommandAsync;
            client.ReactionAdded += ReactionAdded;

            await commands.AddModuleAsync<CommunicationModule>(null);
            await commands.AddModuleAsync<TicketModule>(null);

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
                IResult result = await commands.ExecuteAsync(context, pos, null);
                if (result.IsSuccess && !context.User.IsBot)
                    await UpdateElement(new Tuple<string, string>[] { new Tuple<string, string>("nbMsgs", "1") });
            }
        }

        private async Task UpdateElement(Tuple<string, string>[] elems)
        {
            HttpClient httpClient = new HttpClient();
            var values = new Dictionary<string, string> {
                           { "token", File.ReadAllLines("Keys/websiteToken.dat")[1] },
                           { "action", "add" },
                           { "name", "Orthesia" }
                        };
            foreach (var elem in elems)
            {
                values.Add(elem.Item1, elem.Item2);
            }
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, File.ReadAllLines("Keys/websiteToken.dat")[0]);
            msg.Content = new FormUrlEncodedContent(values);
            
            try
            {
                await httpClient.SendAsync(msg);
            }
            catch (HttpRequestException)
            { }
            catch (TaskCanceledException)
            { }
        }

        private async void UpdateStatus()
        {
            await UpdateElement(new Tuple<string, string>[] { new Tuple<string, string>("serverCount", client.Guilds.Count.ToString()) });
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
