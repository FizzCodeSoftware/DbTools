namespace FizzCode.DbTools.TestBase;

using FizzCode.DbTools.Common;

public static class AssertRowSet
{
    public static void AreEqual(RowSet expected, RowSet actual, SqlEngineVersion version)
    {
        for (var i = 0; i < expected.Count; i++)
        {
            AssertRow.AreEqual(expected[i], actual[i], version);
        }
    }
}