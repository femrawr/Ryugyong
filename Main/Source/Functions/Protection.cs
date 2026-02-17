using Main.Source.Utilities;

namespace Main.Source.Functions
{
    public class Protection
    {
        private static readonly List<List<int>> Usernames = [
            [2, 18, 21, 14, 15], // bruno
            [8, 1, 18, 18, 25, 10, 15, 8, 14, 19, 15, 14], // harry johnson
            [10, 1, 14, 5, 20, 22, 1, 14, 4, 25, 14, 5], // janet van dyne
            [6, 18, 1, 14, 11], // frank
            [22, 1, 12, 9, 21, 19, 5, 18], // valiuser
            [22, 2, 15, 24, 21, 19, 5, 18], // vboxuser
            [10, 15, 8, 14], // john
            [10, 15, 8, 14, 4, 15, 5] // john doe
        ];

        public static bool CheckUsername()
        {
            if (!ConfigProtection.CHECK_USERNAME)
            {
                return false;
            }

            var username = Common.GetLesserCodes(Environment.UserName);

            foreach (var codes in Usernames)
            {
                if (username.SequenceEqual(codes))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CheckDesktopFileNames()
        {
            if (!ConfigProtection.CHECK_DESKTOP_FILE_NAMES)
            {
                return false;
            }

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            string[] files = Directory.GetFiles(desktop);

            if (files.Length == 0)
            {
                return false;
            }

            int badFiles = files.Count((file) => Path
                .GetFileName(file)
                .All((thing) => char.IsUpper(thing) || !char.IsLetter(thing))
            );

            return (double)badFiles / files.Length > 0.8;
        }
    }
}
