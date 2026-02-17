using Discord;
using Discord.Commands;
using NAudio.Wave;

namespace Main.Source.Bot.Commands
{
    public class Audio : CommandBase
    {
        public override string Name => "audio";
        public override string Info => "Plays audio from a file";
        public override string Use => "Path to file (wrap in double quotes)";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length < 1)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string arguments = string.Join(" ", args);

            string path = Utilities.Parser.FromQuotes(arguments);
            if (!File.Exists(path))
            {
                await socket.Message.ReplyAsync($"File `{path}` does not exist");
                return;
            }

            var msg = await socket.Message.ReplyAsync($"Playing audio from `{path}`");

            _ = Task.Run(async () =>
            {
                using var file = new AudioFileReader(path);
                using var wave = new WaveOutEvent();

                wave.Init(file);
                wave.Play();

                while (wave.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(100);
                }

                await msg.ModifyAsync((msg) =>
                {
                    msg.Content = "Finished playing";
                });
            });
        }
    }
}
