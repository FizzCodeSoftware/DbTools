using System;

namespace FizzCode.DbTools.DataGenerator;
public class GeneratorContext(IRandom random, DateTime now)
{
    public IRandom Random { get; } = random;
    public DateTime Now { get; } = now;

    public GeneratorContext(IRandom random)
        : this(random, DateTime.Now)
    {
    }
}
