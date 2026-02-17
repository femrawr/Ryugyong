using Discord;
using Discord.Commands;
using System.Speech.Synthesis;

namespace Main.Source.Bot.Commands
{
    public class TextToSpeech : CommandBase
    {
        public override string Name => "tts";
        public override string Info => "Plays a text-to-speech audio";
        public override string Use => "Anything";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length < 1)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            var speaker = new SpeechSynthesizer();
            speaker.Volume = 100;
            speaker.Speak(string.Join(" ", args));

            await socket.Message.ReplyAsync("Audio played");
        }
    }
}
