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
        public static Program P;

        private string websiteUrl, websiteToken;
        
        public Random rand;
        public DateTime StartTime { private set; get; }

        public Db db { private set; get; }

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
            db = new Db();
            await db.InitAsync();

            P = this;
            rand = new Random();

            if (File.Exists("Keys/websiteToken.dat"))
            {
                string[] website = File.ReadAllLines("Keys/websiteToken.dat");
                websiteUrl = website[0];
                websiteToken = website[1];
            }
            else
            {
                websiteUrl = null;
                websiteToken = null;
            }

            client.MessageReceived += HandleCommandAsync;
            client.ReactionAdded += ReactionAdded;

            await commands.AddModuleAsync<CommunicationModule>(null);
            await commands.AddModuleAsync<TicketModule>(null);

            await client.LoginAsync(TokenType.Bot, File.ReadAllText("Keys/token.dat"));
            StartTime = DateTime.Now;
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
            if (await db.GetCloseMessageId(react.UserId, chan) == react.MessageId.ToString())
            {
                if (react.Emote.Name == "✅")
                {
                    await db.DeleteTicket(react.UserId, ((ITextChannel)chan).Guild);
                }
                else if (react.Emote.Name == "❌")
                {
                    await react.Message.Value.DeleteAsync();
                }
            }
            else if (await db.GetMenuMessageId(react.UserId, chan) == react.MessageId.ToString())
            {
                ITextChannel textChan = (ITextChannel)chan;
                if (react.Emote.Name == "1⃣")
                {
                    await chan.SendMessageAsync(Sentences.category1);
                    await textChan.ModifyAsync(x => x.CategoryId = 585809200282861581);
                }
                else if (react.Emote.Name == "2⃣")
                {
                    await chan.SendMessageAsync(Sentences.category2);
                    await textChan.ModifyAsync(x => x.CategoryId = 484466560204013577);
                }
                else if (react.Emote.Name == "3⃣")
                {
                    await chan.SendMessageAsync(Sentences.category3);
                    await textChan.ModifyAsync(x => x.CategoryId = 585809388221104148);
                }
                else if (react.Emote.Name == "4⃣")
                {
                    await chan.SendMessageAsync(Sentences.category4);
                    await textChan.ModifyAsync(x => x.CategoryId = 585809491493257247);
                }
                else
                    return;
                await (await cach.GetOrDownloadAsync()).DeleteAsync();
                await textChan.AddPermissionOverwriteAsync(await textChan.Guild.GetUserAsync(react.UserId), new OverwritePermissions(viewChannel: PermValue.Allow, sendMessages: PermValue.Allow));
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
            if (websiteUrl == null)
                return;
            HttpClient httpClient = new HttpClient();
            var values = new Dictionary<string, string> {
                           { "token", websiteToken },
                           { "action", "add" },
                           { "name", "Orthesia" }
                        };
            foreach (var elem in elems)
            {
                values.Add(elem.Item1, elem.Item2);
            }
            HttpRequestMessage msg = new HttpRequestMessage(HttpMethod.Post, websiteUrl);
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
