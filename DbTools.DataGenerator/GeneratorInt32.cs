using System;

namespace FizzCode.DbTools.DataGenerator;
public class GeneratorInt32 : GeneratorMinMax<int>
{
    public GeneratorInt32()
        : base(0, int.MaxValue)
    {
    }

    public GeneratorInt32(int min, int max)
        : base(min, max)
    {
    }

    public override object Get()
    {
        // Random next does not include max value, ensure full range
        var value = Context!.Random.Next(-1, Max) + 1;

        return value;
    }
}

public class GeneratorParameterMinMax<T>(Type generatorType)
    : GeneratorParameters(generatorType)
{
}

public class GeneratorParameters(Type generatorType)
{
    public Type GeneratorType { get; } = generatorType;
}
