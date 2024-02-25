
using System.Diagnostics;

namespace TrinityEngineProject
{
    /*
     * Access for:
     *  TgMain - set time, deltatime
     *  User - read time, deltatime
    */

    public static class Time
    {
        static Stopwatch stopwatch = new Stopwatch();
        internal static void Start() { stopwatch.Start(); }

        public static float deltaTime { get; internal set; }
        public static long time => stopwatch.ElapsedMilliseconds;
        public static TimeSpan span => stopwatch.Elapsed;
    }
}
