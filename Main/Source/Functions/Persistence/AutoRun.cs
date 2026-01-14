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

        public static AutoRunStatus Check(string name)
        {
            var run = Registry.CurrentUser.OpenSubKey(RunPath, false);
            if (run == null)
            {
                return AutoRunStatus.FailedOpenAutoRunKey;
            }

            var canRun = run.GetValue(name);
            if (canRun == null)
            {
                return AutoRunStatus.FailedFindAutoRunValue;
            }

            var allowed = Registry.CurrentUser.OpenSubKey(AllowedPath, true);
            if (allowed == null)
            {
                return AutoRunStatus.FailedOpenAllowedKey;
            }

            var canStart = allowed.GetValue(name) as byte[];
            if (canStart == null)
            {
                return AutoRunStatus.AllowedValueMissing;
            }

            if (canStart.Length > 0 && canStart[0] == 2)
            {
                return AutoRunStatus.NotAllowedToStart;
            }

            return AutoRunStatus.AllowedToStart;
        }
    }
}
