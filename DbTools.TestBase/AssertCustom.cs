namespace FizzCode.DbTools.TestBase
{
    using System.Globalization;
    using System.Linq;
    using System.Reflection.Metadata.Ecma335;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public static class AssertCustom
    {
        public static void AreEqual(string expected, string actual, bool ignoreCase = false)
        {
            var areEqual = string.Compare(expected, actual, ignoreCase ? System.StringComparison.OrdinalIgnoreCase : System.StringComparison.Ordinal);
            if (areEqual == 0)
                return;

            var expecteds = expected.Split("\r\n");
            var actuals = actual.Split("\r\n");

            var msg = new StringBuilder("Assert.AreEqual failed. First difference on line ");

            for (var i = 0; i < expecteds.Length; i++)
            {
                areEqual = string.Compare(expecteds[i], actuals[i], ignoreCase ? System.StringComparison.OrdinalIgnoreCase : System.StringComparison.Ordinal);

                if (areEqual == 0)
                    continue;

                var firstDiffIndex = expecteds[i].Zip(actuals[i], (c1, c2) => c1 == c2).TakeWhile(b => b).Count();

                msg.Append(i+1.ToString(CultureInfo.InvariantCulture));
                msg.AppendLine(":");
                msg.Append(' ', firstDiffIndex);
                msg.AppendLine("ˇ");
                msg.AppendLine(expecteds[i]);
                msg.Append(actuals[i]);

                break;
            }

            msg.AppendLine();
            msg.AppendLine();
            msg.AppendLine("Expected followed by actual:");
            msg.AppendLine(expected);
            msg.AppendLine();
            msg.AppendLine(actual);

            throw new AssertFailedException(msg.ToString());
        }
    }
}
