using Discord;
using Discord.Commands;
using System.Text;

namespace Main.Source.Bot.Commands
{
    public class Tree : CommandBase
    {
        public override string Name => "tree";
        public override string Info => "Generates a file tree";
        public override string Use => "Parent dir (wrap in double quotes) + Depth (num)?";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length < 1)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string input = string.Join(" ", args);

            int start = input.IndexOf('"');
            int end = input.LastIndexOf('"');

            if (start == -1 || end == -1 || end <= start)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string path = input.Substring(start + 1, end - start - 1);
            if (!Directory.Exists(path))
            {
                await socket.Message.ReplyAsync($"Directory `{path}` does not exist");
                return;
            }

            string depthStr = input.Substring(end + 1).Trim();
            if (!int.TryParse(depthStr, out int maxDepth))
            {
                maxDepth = 1;
            }

            var tree = new StringBuilder();

            Utilities.Files.FileTree(path, "", 0, maxDepth, tree);

            string output = tree.ToString();

            if (output.Length > 1900 + path.Length + 2)
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(output)))
                {
                    await socket.Channel.SendFileAsync(
                        stream: stream,
                        filename: "tree.txt",
                        text: $"`{path}`",
                        messageReference: new MessageReference(socket.Message.Id)
                    );
                }
            }
            else
            {
                await socket.Message.ReplyAsync($"`{path}`\n```\n{output}\n```");
            }
        }
    }
}
