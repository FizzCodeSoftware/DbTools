namespace FizzCode.DbTools.DataGenerator
{
    public abstract class GeneratorMinMaxLength<T> : GeneratorBase<T>
    {
        public int MinLength { get; private set; }
        public int MaxLength { get; private set; }
        public GeneratorMinMaxLength(int minLength, int maxLength) : base()
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }
    }
}
