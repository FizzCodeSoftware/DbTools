namespace FizzCode.DbTools.TestBase;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using FizzCode.DbTools.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public static class AssertRow
{
    public static void AreEqual(Row expected, Row actual, SqlEngineVersion version)
    {
        if (!Compare(expected, actual, version, out var message))
        { 
            throw new AssertFailedException("AssertRow.AreEqual failed. " + message.ToString());
        }
    }

    public static bool Compare(Row expected, Row actual, SqlEngineVersion version, out string message)
    {
        var sb = new StringBuilder("First difference is on row number ");

        var expectedKeys = expected.Keys.ToList();

        var missingKeys = new List<string>();
        var additionalKeys = new List<string>();

        var isEqualSoFar = true;
        var i = 0;

        foreach (var expectedKey in expectedKeys)
        {
            if (!actual.ContainsKey(expectedKey))
            {
                missingKeys.Add(expectedKey);
            }
            else
            {
                var expectedValue = expected[expectedKey];
                var actualValue = actual[expectedKey];

                bool isNotEqual;

                if (version is SqLiteVersion
                    && expectedValue.GetType() != actualValue.GetType())
                    isNotEqual = !expectedValue.Equals(Convert.ChangeType(actualValue, expectedValue.GetType()));
                else
                    isNotEqual = !expectedValue.Equals(actualValue);

                if (isNotEqual)
                {
                    if (isEqualSoFar)
                    {
                        isEqualSoFar = false;
                        sb.Append((i + 1).ToString(CultureInfo.InvariantCulture));
                        sb.AppendLine(":");
                    }

                    sb.Append(expectedKey);
                    sb.Append(": ");
                    sb.Append(expectedValue);
                    sb.Append(", ");
                    sb.Append(expectedKey);
                    sb.Append(": ");
                    sb.Append(actualValue);
                    sb.AppendLine();
                }
            }

            i++;
        }

        if (isEqualSoFar && missingKeys.Count == 0 && additionalKeys.Count == 0)
        {
            message = null;
            return true;
        }

        sb.AppendLine();
        sb.AppendLine();
        sb.AppendLine("Expected followed by actual:");

        foreach (var expectedKvp in expected)
        {
            sb.Append(expectedKvp.Key);
            sb.Append(": ");
            sb.Append(expectedKvp.Value);
            sb.AppendLine();
        }
        foreach (var actualKvp in actual)
        {
            sb.Append(actualKvp.Key);
            sb.Append(": ");
            sb.Append(actualKvp.Value);
            sb.AppendLine();
        }

        message = sb.ToString();
        return false;
    }
}
