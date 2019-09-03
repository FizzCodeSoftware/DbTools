using System;

namespace FizzCode.DbTools.DataGenerator
{
    public class GeneratorInt32 : GeneratorMinMax<int>
    {
        public GeneratorInt32() : base(0, int.MaxValue)
        { }

        public GeneratorInt32(int min, int max) : base(min, max)
        {
        }

        public override object Get()
        {
            // Random next does not include max value, ensure full range
            var value = Context.Random.Next(-1, Max) + 1;

            return value;
        }
    }

    public static class GeneratorFactory
    {
    }

    public class GeneratorParameterMinMax<T> : GeneratorParameters
    {
        public GeneratorParameterMinMax(Type generatorType) : base(generatorType)
        {
        }
    }

    public class GeneratorParameters
    {
        public Type GeneratorType { get; }

        public GeneratorParameters(Type generatorType)
        {
            GeneratorType = generatorType;
        }
    }
}
