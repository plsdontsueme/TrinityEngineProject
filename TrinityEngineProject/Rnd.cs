using System;

namespace TrinityEngineProject
{
    public static class Rnd
    {
        static Random random = new Random();
        public static float RandomRange(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }
        public static int RandomRangeInt(int min, int max)
        {
            return (random.Next() * (max - min) + min);
        }

        public static void SetSeed(int seed)
        {
            random = new Random(seed);
        }
    }
}
