using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
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

            client.MessageReceived += HandleCommandAsync;
            client.ReactionAdded += ReactionAdded;

            await commands.AddModuleAsync<CommunicationModule>();
            await commands.AddModuleAsync<TicketModule>();

            await client.LoginAsync(TokenType.Bot, File.ReadAllText("Keys/token.dat"));
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task ReactionAdded(Cacheable<IUserMessage, ulong> cach, ISocketMessageChannel chan, SocketReaction react)
        {
            if (File.Exists("Saves/support-" + react.UserId + ".dat") && File.ReadAllText("Saves/support-" + react.UserId + ".dat") == react.MessageId.ToString())
            {
                if (react.Emote.Name == "✅")
                {
                    File.Delete("Saves/support-" + react.UserId + ".dat");
                    File.WriteAllText("Saves/timer-" + react.UserId + ".dat", DateTime.Now.ToString("yyMMddHHmmss"));
                    await (chan as ITextChannel).DeleteAsync();
                }
                else if (react.Emote.Name == "❌")
                {
                    File.Delete("Saves/support-" + react.UserId + ".dat");
                    await react.Message.Value.DeleteAsync();
                }
            }
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null) return;

            int pos = 0;
            if (msg.HasMentionPrefix(client.CurrentUser, ref pos) || msg.HasStringPrefix("o.", ref pos))
            {
                var context = new SocketCommandContext(client, msg);
                await commands.ExecuteAsync(context, pos);
            }
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
