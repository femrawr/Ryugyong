using Discord;
using Discord.Commands;

namespace Main.Source.Bot.Commands
{
    public class Messagebox : CommandBase
    {
        public override string Name => "msgbox";
        public override string Info => "Opens a messagebox on the users screen";
        public override string Use => "=text + =caption?";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            
        }
    }
}
