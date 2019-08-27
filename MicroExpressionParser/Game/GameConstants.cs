
namespace RPGEngine.Game
{
    public class GameConstants
    {
        public const long TickTime = 1000;
    }

    public static class ConfigFiles
    {
        public const string BASE_PATH = "config";
        public const string EXENSION = ".json";

        public const string CORE = "core";
        public const string ATTRIBUTES = "attributes";
        public const string BASE_VALUES = "base_values";
        public const string RESOURCES = "resources";
        public const string DAMAGE_TYPES = "damage_types";
        public const string CLASSES = "classes";
        public const string STATUSES = "statuses";
        public const string SKILLS = "skills";
        public const string STATUS = "stats";

    }

    public static class CommandsConstants
    {
        public const string CREATE_COMMAND = "create";

        public const string TARGET_COMMAND = "target";

        public const string ATTACK_COMMAND = "attack";
    }
}
