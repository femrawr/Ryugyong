using Microsoft.Win32;

namespace Main.Source.Functions.Persistence
{
    public class AutoRun
    {
        private const string AllowedPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StartupApproved\\Run";
        private const string RunPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Run";

        public static void Install(string name, string path)
        {
            var allowed = Registry.CurrentUser.OpenSubKey(AllowedPath, true);
            if (allowed == null)
            {
                Utilities.Logger.Warn("AutoRun.Install: failed to open start approved");
            }
            else
            {
                allowed.DeleteValue(name, false);
            }

            var run = Registry.CurrentUser.OpenSubKey(RunPath, true);
            if (run == null)
            {
                Utilities.Logger.Warn("AutoRun.Install: failed to open run");
            }
            else
            {
                try
                {
                    run.SetValue(name, $"\"{path}\" {ConfigMisc.LAUNCH_KEY}");
                }
                catch
                {
                    Utilities.Logger.Warn($"AutoRun.Install: failed to install");
                }
            }
        }

        public static void Uninstall(string name)
        {
            var run = Registry.CurrentUser.OpenSubKey(RunPath, true);
            if (run == null)
            {
                Utilities.Logger.Warn("AutoRun.Uninstall: failed to open run");
            }
            else
            {
                try
                {
                    run.DeleteValue(name, true);
                }
                catch
                {
                    Utilities.Logger.Warn($"AutoRun.Install: failed to uninstall");
                }
            }
        }

        public static string Check(string name)
        {
            var run = Registry.CurrentUser.OpenSubKey(RunPath, false);
            if (run == null)
            {
                return "Failed to open autorun key";
            }

            var canRun = run.GetValue(name);
            if (canRun == null)
            {
                return "Failed to find file";
            }

            var allowed = Registry.CurrentUser.OpenSubKey(AllowedPath, true);
            if (allowed == null)
            {
                return "Failed to open startup allowed key";
            }

            var canStart = allowed.GetValue(name) as byte[];
            if (canStart == null)
            {
                return "File is missing from startup allowed";
            }

            if (canStart.Length > 0 && canStart[0] == 2)
            {
                return "File is not allowed to auto start";
            }

            return "All is good";
        }
    }
}
