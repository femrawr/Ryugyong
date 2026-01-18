using System.Security.Principal;

namespace Main.Source.Utilities
{
    public class Common
    {
        public static List<int> GetLesserCodes(string data)
        {
            var codes = new List<int>();

            foreach (char thing in data.ToLower())
            {
                if (!char.IsLetter(thing))
                {
                    continue;
                }

                int code = thing - (int)'a' + 1;
                codes.Add(code);
            }

            return codes;
        }

        public static bool IsElevated()
        {
            using var inden = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(inden);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
