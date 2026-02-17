namespace Main.Source.Utilities
{
    public class Windows
    {
        public enum WindowsVersion
        {
            Windows11,
            Windows10,
            Windows8,
            Windows7,
            Unknown
        }

        public static WindowsVersion GetWindowsVersion()
        {
            var version = Environment.OSVersion.Version;

            if (version.Major == 10 && version.Build >= 22000)
            {
                return WindowsVersion.Windows11;
            }

            if (version.Major == 10)
            {
                return WindowsVersion.Windows10;
            }

            if (version.Major == 6 && (version.Minor == 2 || version.Minor == 3))
            {
                return WindowsVersion.Windows8;
            }

            if (version.Major == 6 && version.Minor >= 1)
            {
                return WindowsVersion.Windows7;
            }

            return WindowsVersion.Unknown;
        }
    }
}
