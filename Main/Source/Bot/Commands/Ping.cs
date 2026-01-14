using Discord;
using Discord.Commands;

namespace Main.Source.Bot.Commands
{
    public class Ping : CommandBase
    {
        public override string Name => "ping";
        public override string Info => "Test command";
        public override string Use => "No usage";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            await socket.Message.ReplyAsync("pong");
        }
    }
}
