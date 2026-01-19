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
        /// <summary>
        /// `targetPath` is the path of the file or command
        /// that will be opened or executed. Basically the
        /// contents of the "Target" textbox in the
        /// properties of the item.
        /// 
        /// <br />
        /// <br />
        /// 
        /// `filePath` is the path of the shortcut that
        /// will be created. No ".lnk" extension is needed.
        /// 
        /// <br />
        /// <br />
        /// 
        /// `openIn` is the path of the directory `target`
        /// will be opened or executed in.
        /// 
        /// <br />
        /// <br />
        /// 
        /// `icon` is the path of another file in which
        /// the shortcut's icon will be the same as.
        /// </summary>
        public static void CreateShortcut(
            string target,
            string filePath,
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
            dynamic shortcut = shell.CreateShortcut(filePath + ".lnk");

            shortcut.TargetPath = target;
            shortcut.WorkingDirectory = openIn;
            shortcut.Arguments = args;
            shortcut.IconLocation = icon;
            shortcut.Description = desc;
            shortcut.Hotkey = keybind;
            shortcut.Save();
        }

        /// <summary>
        /// `filePath` is the path of the shortcut that
        /// will be overwritten.
        /// 
        /// <br />
        /// <br />
        /// 
        /// `target` is the path of the file or command
        /// that will be opened or executed. Basically the
        /// contents of the "Target" textbox in the
        /// properties of the item.
        /// 
        /// `openIn` is the path of the directory `target`
        /// will be opened or executed in.
        /// 
        /// <br />
        /// <br />
        /// 
        /// `icon` is the path of another file in which
        /// the shortcut's icon will be the same as.
        /// </summary>
        public static void OverwriteShortcut(
            string filePath,
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
            dynamic shortcut = shell.CreateShortcut(filePath);

            shortcut.TargetPath = target ?? shortcut.TargetPath;
            shortcut.WorkingDirectory = openIn ?? shortcut.WorkingDirectory;
            shortcut.Arguments = args ?? shortcut.Arguments;
            shortcut.IconLocation = icon ?? shortcut.IconLocation;
            shortcut.Description = desc ?? shortcut.Description;
            shortcut.Hotkey = keybind ?? shortcut.Hotkey;
            shortcut.Save();
        }

        public static ShortcutInfo? GetShortcutInfo(string filePath)
        {
            var com = Type.GetTypeFromProgID("WScript.shell");
            if (com == null)
            {
                Logger.Error("GetShortcutInfo: failed to get wscript com object");
                return null;
            }

            dynamic shell = Activator.CreateInstance(com);
            dynamic shortcut = shell.CreateShortcut(filePath);

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
