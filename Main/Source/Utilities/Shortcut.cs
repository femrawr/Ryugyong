namespace Main.Source.Utilities
{
    public class ShortcutInfo
    {
        public string Target { get; set; }
        public string Args { get; set; }
        public string Icon { get; set; }
        public string Desc { get; set; }
        public string OpenIn { get; set; }
        public string Keybind { get; set; }
    }

    public class Shortcut
    {
        public static void CreateShortcut(
            string from,
            string to,
            string openIn = "",
            string args = "",
            string icon = "",
            string desc = "",
            string keybind = ""
        )
        {
            var com = Type.GetTypeFromProgID("WScript.shell");
            if (com == null)
            {
                Logger.Error("CreateShortcut: failed to get wscript com object");
                return;
            }

            dynamic shell = Activator.CreateInstance(com);

            dynamic shortcut = shell.CreateShortcut(to + ".lnk");
            shortcut.TargetPath = from;
            shortcut.WorkingDirectory = openIn;
            shortcut.Arguments = args;
            shortcut.IconLocation = icon;
            shortcut.Description = desc;
            shortcut.Hotkey = keybind;
            shortcut.Save();
        }

        public static void OverwriteShortcut(
            string path,
            string? target = null,
            string? openIn = null,
            string? args = null,
            string? icon = null,
            string? desc = null,
            string? keybind = null
        )
        {
            var com = Type.GetTypeFromProgID("WScript.shell");
            if (com == null)
            {
                Logger.Error("OverwriteShortcut: failed to get wscript com object");
                return;
            }

            dynamic shell = Activator.CreateInstance(com);

            dynamic shortcut = shell.CreateShortcut(path);

            shortcut.TargetPath = target ?? shortcut.TargetPath;
            shortcut.WorkingDirectory = openIn ?? shortcut.WorkingDirectory;
            shortcut.Arguments = args ?? shortcut.Arguments;
            shortcut.IconLocation = icon ?? shortcut.IconLocation;
            shortcut.Description = desc ?? shortcut.Description;
            shortcut.Hotkey = keybind ?? shortcut.Hotkey;
            shortcut.Save();
        }

        public static ShortcutInfo? GetShortcutInfo(string path)
        {
            var com = Type.GetTypeFromProgID("WScript.shell");
            if (com == null)
            {
                Logger.Error("GetShortcutInfo: failed to get wscript com object");
                return null;
            }

            dynamic shell = Activator.CreateInstance(com);

            dynamic shortcut = shell.CreateShortcut(path);

            return new ShortcutInfo
            {
                Target = shortcut.TargetPath,
                Args = shortcut.Arguments,
                Icon = shortcut.IconLocation,
                Desc = shortcut.Description,
                OpenIn = shortcut.WorkingDirectory,
                Keybind = shortcut.Hotkey,
            };
        }
    }
}
