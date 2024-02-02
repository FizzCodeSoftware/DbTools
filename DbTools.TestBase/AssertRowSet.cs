namespace FizzCode.DbTools.TestBase;

using System.Text;
using FizzCode.DbTools.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public static class AssertRowSet
{
    public static void AreEqual(RowSet expected, RowSet actual, SqlEngineVersion version)
    {
        var sb = new StringBuilder();
        var isEqual = true;
        for (var i = 0; i < expected.Count; i++)
        {
            isEqual = AssertRow.Compare(expected[i], actual[i], version, out var message) && isEqual;
            sb.Append(message);
        }

        if (!isEqual)
            throw new AssertFailedException("AssertRowSet.AreEqual failed. " + sb.ToString());
    }
}