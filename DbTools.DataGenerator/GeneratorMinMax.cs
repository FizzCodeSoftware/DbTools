namespace FizzCode.DbTools.DataGenerator
{
    public abstract class GeneratorMinMax<T> : GeneratorBase<T>
    {
        public GeneratorMinMax(T min, T max) : base()
        {
            Min = min;
            Max = max;
        }

        public T Min { get; private set; }
        public T Max { get; private set; }
    }
}
