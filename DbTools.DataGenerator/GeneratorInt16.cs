namespace FizzCode.DbTools.DataGenerator
{
    public class GeneratorInt16 : GeneratorMinMax<short>
    {
        public GeneratorInt16() : base(0, short.MaxValue)
        {
        }

        public GeneratorInt16(short min, short max) : base(min, max)
        {
        }

        public override object Get()
        {
            var value = Context.Random.Next(Min, Max + 1);
            return (short)value;
        }
    }
}
