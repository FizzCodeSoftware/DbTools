namespace FizzCode.DbTools.DataGenerator
{
    using System;

    public class GeneratorContext
    {
        public IRandom Random { get; }
        public DateTime Now { get; }

        public GeneratorContext(IRandom random)
            : this(random, DateTime.Now)
        {
        }

        public GeneratorContext(IRandom random, DateTime now)
        {
            Random = random;
            Now = now;
        }
    }
}
