namespace Main.Source.Utilities
{
    public class Parser
    {
        public static string FromQuotes(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            int start = text.IndexOf('"');
            int end = text.LastIndexOf('"');

            if (start == -1 || end == -1 || end <= start)
            {
                return string.Empty;
            }

            return text.Substring(start + 1, end - start - 1);
        }

        public static string ExactFromQuotes(string text, string flag)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(flag))
            {
                return string.Empty;
            }

            string pattern = flag + "=\"";

            int start = text.IndexOf(pattern);
            if (start == -1)
            {
                return string.Empty;
            }

            start += pattern.Length;
            int end = text.IndexOf('"', start);
            if (end == -1)
            {
                return string.Empty;
            }

            return text.Substring(start, end - start);
        }

        public static bool GetFlag(string text, string flag)
        {
            if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(flag))
            {
                return false;
            }

            string[] split = text.Split(" ");
            return split.Any((thing) => thing == "-" + flag);
        }
    }
}
