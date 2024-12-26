using System;

namespace DragonAPI.Helpers
{
    public class StaticParams
    {
        public static DateTime DateTimeNow => DateTime.UtcNow;
        public static DateTime DateTimeMin => DateTime.MinValue;
        public static bool EnableExternalService = true;

        public static int MAX_COMBINATION_TIME = 7;
        public static int MAX_INCUBATE_INDEX = 7;
        public static int MAX_DEGENERATION_INDEX = 7;
    }
}
