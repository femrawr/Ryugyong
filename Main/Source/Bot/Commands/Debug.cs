using Discord;
using Discord.Commands;

namespace Main.Source.Bot.Commands
{
    public class Debug : CommandBase
    {
        public override string Name => "debug";
        public override string Info => "Debug command";
        public override string Use => "[-checkpersistence | -dumplogs]";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length < 1)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string arguments = string.Join(" ", args);


        }
    }
}
