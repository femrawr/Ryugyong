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

            string arguments = string.Join(" ", args);

            string path = Utilities.Parser.FromQuotes(arguments);
            if (!Directory.Exists(path))
            {
                await socket.Message.ReplyAsync($"Directory `{path}` does not exist");
                return;
            }

            string depthStr = arguments.Replace($"\"{path}\"", "").Trim();
            if (!int.TryParse(depthStr, out int maxDepth))
            {
                maxDepth = 1;
            }

            var builder = new StringBuilder();
            Utilities.Files.FileTree(path, "", 0, maxDepth, builder);

            string tree = builder.ToString();

            if (tree.Length > 1900 + path.Length + 2)
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(tree)))
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
                await socket.Message.ReplyAsync($"`{path}`\n```\n{tree}\n```");
            }
        }
    }
}
