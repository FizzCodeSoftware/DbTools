using System.Linq;

namespace FizzCode.DbTools.DataGenerator;
public class GeneratorString : GeneratorMinMaxLength<string>
{
    public GeneratorString(int minLength, int maxLength)
        : base(minLength, maxLength)
    {
    }

    public override object Get()
    {
        var chars = Enumerable.Range(0, MaxLength - MinLength).Select(_ => (char)Context.Random.Next('a', 'z' + 1)).ToArray();
        return new string(chars);
    }
}
