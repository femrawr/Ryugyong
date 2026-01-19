using Discord;
using Discord.Commands;

namespace Main.Source.Bot.Commands
{
    public class Download : CommandBase
    {
        private static readonly HttpClient httpClient = new();

        public override string Name => "download";
        public override string Info => "Downloads a file from the server to the device";
        public override string Use => "Reply to file | File url | File as attachment";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            string url = string.Empty;
            string name = string.Empty;

            if (socket.Message.Attachments.Any())
            {
                var attachment = socket.Message.Attachments.First();

                url = attachment.Url;
                name = attachment.Filename;
            }

            if (url == "" && args.Length > 0)
            {
                if (!Uri.TryCreate(args[0], UriKind.Absolute, out var uri))
                {
                    await socket.Message.ReplyAsync("Usage: " + Use);
                    return;
                }

                if (uri.Scheme != Uri.UriSchemeHttp || uri.Scheme != Uri.UriSchemeHttps)
                {
                    await socket.Message.ReplyAsync("Usage: " + Use);
                    return;
                }

                url = args[0];
                name = Path.GetFileName(url);
            }

            if (url == "" && socket.Message.Reference != null)
            {
                var repliedMessage = await socket.Channel.GetMessageAsync(socket.Message.Reference.MessageId.Value);
                if (repliedMessage.Attachments.Any())
                {
                    var attachment = repliedMessage.Attachments.First();

                    url = attachment.Url;
                    name = attachment.Filename;
                }
            }

            if (url == "")
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string path = Path.Join(Path.GetTempPath(), name);

            using (var res = await httpClient.GetAsync(url))
            {
                res.EnsureSuccessStatusCode();

                await using (var stream = new FileStream(
                    path,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None
                ))
                {
                    await res.Content.CopyToAsync(stream);

                    await socket.Message.ReplyAsync($"Downloaded to:\n```{path}\n```");
                }
            }
        }
    }
}
