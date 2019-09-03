namespace FizzCode.DbTools.DataGenerator
{
    public abstract class GeneratorMinMaxLength<T> : GeneratorBase<T>
    {
        public int MinLength { get; }
        public int MaxLength { get; }

        protected GeneratorMinMaxLength(int minLength, int maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }
    }
}
