namespace Main.Source
{
    public enum PersistMethod
    {
        AutoRunRegKey,
        StartFolder,
        WinLogonRegKey,
        TaskScheduler,
        ImpersonateLNK
    }

    public class ConfigMisc
    {
        public const bool DEBUG_MODE = false;

        public const string CRYPTO_KEY = "";

        public const string LAUNCH_KEY = "";
    }

    public class ConfigProtection
    {
        public const bool CHECK_USERNAME = false;
        public const bool CHECK_DESKTOP_FILE_NAMES = false;
    }

    public class ConfigOptions
    {
        public const bool REQUIRE_ADMIN = false;
        public const bool PROMPT_ADMIN = true;
        public const bool FORCE_ADMIN = true;
        public const bool CONTINUE_WITHOUT_ADMIN = true;

        public const bool USE_MUTEX = true;
        public const string MUTEX_NAME = "";

        public const bool ADMIN_CUSTOM_PERSISTENCE = true;
        public const PersistMethod PERSISTENCE_METHOD = 0;
    }

    public class ConfigSetup
    {
        public const bool USE_NEW_DIR = false;
        public const string NEW_DIR = "";

        public const bool USE_NEW_NAME = false;
        public const string NEW_NAME = "";
    }

    public class ConfigBot
    {
        public const string BOT_TOKEN = "";
        public const string SERVER_ID = "";
        public const string CATEGORY_ID = "";

        public const string COMMAND_PREFIX = ".";
    }

    public class ConfigVersion
    {
        public const int MAJOR = 2;
        public const int MINOR = 9;
        public const int PATCH = 5;

        public const string TRACKING = "";
    }
}
