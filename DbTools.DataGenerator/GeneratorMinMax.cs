namespace FizzCode.DbTools.DataGenerator;

public abstract class GeneratorMinMax<T> : GeneratorBase<T>
{
    protected GeneratorMinMax(T min, T max)
    {
        Min = min;
        Max = max;
    }

    public T Min { get; private set; }
    public T Max { get; private set; }
}
