namespace FizzCode.DbTools.DataGenerator
{
    using System;

    public class RandomBasic : IRandom
    {
        private readonly Random Random;

        public RandomBasic(int seed)
        {
            Random = new Random(seed);
        }

        public int Next()
        {
            return Random.Next();
        }

        public int Next(int maxValue = int.MaxValue)
        {
            return Random.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }
    }
}
